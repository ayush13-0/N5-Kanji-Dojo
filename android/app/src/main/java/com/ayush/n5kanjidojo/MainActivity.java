package com.ayush.n5kanjidojo;

import android.app.Activity;
import android.content.Intent;
import android.content.res.AssetManager;
import android.net.Uri;
import android.os.Bundle;
import android.speech.tts.TextToSpeech;
import android.view.ViewGroup;
import android.webkit.JavascriptInterface;
import android.webkit.WebResourceError;
import android.webkit.WebResourceRequest;
import android.webkit.WebResourceResponse;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.TextView;

import java.io.IOException;
import java.io.InputStream;
import java.util.Locale;

/**
 * N5 Kanji Dojo — fully offline WebView shell (v1.1, dependency-free).
 *
 * Changes from v1.0: the androidx.webkit asset loader is gone. App files are
 * served straight from the AssetManager by this WebViewClient, so the app
 * has zero library dependencies. Startup is guarded, speech is initialised
 * lazily, and any load failure shows the real error on screen (with one
 * automatic fallback to a direct asset load) instead of a blank page.
 *
 * Security model is unchanged: the APK declares no permissions (not even
 * INTERNET), web content gets no file/content access unless the one-time
 * fallback fires, and real web links open in the system browser.
 */
public class MainActivity extends Activity {

    private static final String APP_HOST = "appassets.androidplatform.net";
    private static final String HOME_URL =
            "https://" + APP_HOST + "/assets/www/index.html";
    private static final String FALLBACK_URL =
            "file:///android_asset/www/index.html";

    private WebView webView;
    private boolean fallbackTried = false;

    // ---- Text to speech (created lazily on first use) ----
    private TextToSpeech tts;
    private boolean ttsReady = false;
    private String pendingText = null;
    private float pendingRate = 1.0f;

    private synchronized void ensureTts() {
        if (tts != null) return;
        try {
            tts = new TextToSpeech(getApplicationContext(), status -> {
                if (status == TextToSpeech.SUCCESS) {
                    try {
                        int res = tts.setLanguage(Locale.JAPAN);
                        if (res == TextToSpeech.LANG_MISSING_DATA
                                || res == TextToSpeech.LANG_NOT_SUPPORTED) {
                            tts.setLanguage(Locale.getDefault());
                        }
                    } catch (Exception ignored) { }
                    ttsReady = true;
                    if (pendingText != null) {
                        speakNow(pendingText, pendingRate);
                        pendingText = null;
                    }
                }
            });
        } catch (Exception ignored) { }
    }

    private void speakNow(String text, float rate) {
        try {
            if (tts == null || !ttsReady) return;
            float r = rate;
            if (r < 0.5f) r = 0.5f;
            if (r > 2.0f) r = 2.0f;
            tts.setSpeechRate(r);
            tts.speak(text, TextToSpeech.QUEUE_FLUSH, null, "dojo-utterance");
        } catch (Exception ignored) { }
    }

    /** JavaScript bridge backing the speechSynthesis polyfill. */
    public class TtsBridge {
        @JavascriptInterface
        public void speak(final String text, final double rate) {
            if (text == null) return;
            final String trimmed = text.trim();
            if (trimmed.isEmpty() || trimmed.length() > 500) return;
            runOnUiThread(() -> {
                ensureTts();
                if (ttsReady) {
                    speakNow(trimmed, (float) rate);
                } else {
                    pendingText = trimmed;
                    pendingRate = (float) rate;
                }
            });
        }

        @JavascriptInterface
        public void stop() {
            runOnUiThread(() -> {
                try {
                    if (tts != null && ttsReady) tts.stop();
                } catch (Exception ignored) { }
            });
        }
    }

    private static final String TTS_POLYFILL =
            "(function(){" +
            "if(window.__dojoTtsPolyfill)return;window.__dojoTtsPolyfill=true;" +
            "var keep=false;" +
            "try{var n=window.speechSynthesis;" +
            "if(n&&typeof n.speak==='function'){var v=n.getVoices?n.getVoices():[];" +
            "if(v&&v.length)keep=true;}}catch(e){}" +
            "if(keep||!window.DojoTTS)return;" +
            "function Utt(t){this.text=t||'';this.lang='ja-JP';this.rate=1;" +
            "this.pitch=1;this.volume=1;this.voice=null;this.onstart=null;" +
            "this.onend=null;this.onerror=null;this.onboundary=null;}" +
            "try{window.SpeechSynthesisUtterance=Utt;}catch(e){}" +
            "var synth={speaking:false,pending:false,paused:false,onvoiceschanged:null," +
            "speak:function(u){try{var t=(u&&u.text)||'';var r=(u&&u.rate)||1;" +
            "window.DojoTTS.speak(t,r);" +
            "if(u&&u.onstart)setTimeout(function(){u.onstart({});},10);" +
            "if(u&&u.onend)setTimeout(function(){u.onend({});}," +
            "Math.max(500,t.length*160/Math.max(r,0.5)));}catch(e){}}," +
            "cancel:function(){try{window.DojoTTS.stop();}catch(e){}}," +
            "pause:function(){},resume:function(){}," +
            "getVoices:function(){return[{name:'Japanese (device TTS)'," +
            "lang:'ja-JP',default:true,localService:true,voiceURI:'dojo-device-ja'}];}," +
            "addEventListener:function(){},removeEventListener:function(){}," +
            "dispatchEvent:function(){return true;}};" +
            "try{Object.defineProperty(window,'speechSynthesis'," +
            "{value:synth,configurable:true});}catch(e){window.speechSynthesis=synth;}" +
            "})();";

    // ---- Asset serving (replaces the androidx WebViewAssetLoader) ----
    private WebResourceResponse serveAsset(Uri url) {
        String path = url.getPath();
        if (path == null) return null;
        String assetPath;
        if (path.startsWith("/assets/")) {
            assetPath = path.substring("/assets/".length());
        } else if (path.equals("/assets") || path.equals("/")) {
            assetPath = "www/index.html";
        } else {
            return null;
        }
        if (assetPath.isEmpty() || assetPath.endsWith("/")) {
            assetPath += "index.html";
        }
        AssetManager am = getAssets();
        try {
            InputStream is = am.open(assetPath);
            return new WebResourceResponse(mimeFor(assetPath), "UTF-8", is);
        } catch (IOException e) {
            // SPA-style fallback: unknown in-app paths get the app shell.
            if (!assetPath.contains(".")) {
                try {
                    InputStream is = am.open("www/index.html");
                    return new WebResourceResponse("text/html", "UTF-8", is);
                } catch (IOException ignored) { }
            }
            return null;
        }
    }

    private static String mimeFor(String path) {
        String p = path.toLowerCase(Locale.ROOT);
        if (p.endsWith(".html") || p.endsWith(".htm")) return "text/html";
        if (p.endsWith(".js") || p.endsWith(".mjs")) return "text/javascript";
        if (p.endsWith(".css")) return "text/css";
        if (p.endsWith(".json")) return "application/json";
        if (p.endsWith(".png")) return "image/png";
        if (p.endsWith(".jpg") || p.endsWith(".jpeg")) return "image/jpeg";
        if (p.endsWith(".svg")) return "image/svg+xml";
        if (p.endsWith(".webp")) return "image/webp";
        if (p.endsWith(".gif")) return "image/gif";
        if (p.endsWith(".ico")) return "image/x-icon";
        if (p.endsWith(".txt") || p.endsWith(".csv")) return "text/plain";
        if (p.endsWith(".woff2")) return "font/woff2";
        if (p.endsWith(".woff")) return "font/woff";
        if (p.endsWith(".ttf")) return "font/ttf";
        if (p.endsWith(".mp3")) return "audio/mpeg";
        return "application/octet-stream";
    }

    private void showErrorPage(String title, String detail) {
        if (webView == null) return;
        String html = "<!doctype html><meta charset='utf-8'>"
                + "<meta name='viewport' content='width=device-width,initial-scale=1'>"
                + "<body style='font-family:sans-serif;background:#12183c;color:#fff7e6;"
                + "padding:24px;line-height:1.5'>"
                + "<h2 style='margin-top:0'>N5 Kanji Dojo hit a problem</h2>"
                + "<p>" + title + "</p>"
                + "<p style='opacity:.75;font-size:14px'>" + detail + "</p>"
                + "<p><a href='" + HOME_URL + "' style='display:inline-block;"
                + "background:#e65a46;color:#fff;padding:12px 20px;border-radius:10px;"
                + "text-decoration:none'>Tap to retry</a></p>"
                + "<p style='opacity:.6;font-size:13px'>If this stays, send a screenshot "
                + "of this screen — the message above says exactly what went wrong.</p>"
                + "</body>";
        try {
            webView.loadDataWithBaseURL(null, html, "text/html", "UTF-8", null);
        } catch (Exception ignored) { }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        try {
            startWebView();
        } catch (Throwable t) {
            TextView tv = new TextView(this);
            tv.setText("N5 Kanji Dojo could not start.\n\n"
                    + t.getClass().getSimpleName() + ": " + t.getMessage()
                    + "\n\nPlease send a screenshot of this screen.");
            tv.setTextSize(16);
            tv.setPadding(48, 48, 48, 48);
            setContentView(tv);
        }
    }

    private void startWebView() {
        webView = new WebView(this);
        webView.setLayoutParams(new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.MATCH_PARENT));
        setContentView(webView);

        // Android 15+ draws apps edge-to-edge: keep the app's content clear
        // of the status bar, navigation bar and display cutout.
        if (android.os.Build.VERSION.SDK_INT >= 30) {
            webView.setOnApplyWindowInsetsListener((v, insets) -> {
                android.graphics.Insets bars = insets.getInsets(
                        android.view.WindowInsets.Type.systemBars()
                                | android.view.WindowInsets.Type.displayCutout());
                v.setPadding(bars.left, bars.top, bars.right, bars.bottom);
                return insets;
            });
        }

        WebSettings s = webView.getSettings();
        s.setJavaScriptEnabled(true);          // the study app needs JavaScript
        s.setDomStorageEnabled(true);          // localStorage: saved progress
        s.setTextZoom(100);                    // keep the designed layout size
                                               // regardless of system font size
        s.setAllowFileAccess(false);           // enabled only if fallback fires
        s.setAllowContentAccess(false);
        s.setAllowFileAccessFromFileURLs(false);
        s.setAllowUniversalAccessFromFileURLs(false);
        s.setMixedContentMode(WebSettings.MIXED_CONTENT_NEVER_ALLOW);
        s.setGeolocationEnabled(false);
        s.setSaveFormData(false);
        s.setMediaPlaybackRequiresUserGesture(true);
        s.setSupportZoom(false);
        s.setBuiltInZoomControls(false);
        s.setDisplayZoomControls(false);
        WebView.setWebContentsDebuggingEnabled(false);

        webView.addJavascriptInterface(new TtsBridge(), "DojoTTS");

        webView.setWebViewClient(new WebViewClient() {
            @Override
            public WebResourceResponse shouldInterceptRequest(WebView view,
                                                              WebResourceRequest request) {
                Uri url = request.getUrl();
                if (url != null && APP_HOST.equals(url.getHost())) {
                    return serveAsset(url);
                }
                return null;
            }

            @Override
            @SuppressWarnings("deprecation")
            public WebResourceResponse shouldInterceptRequest(WebView view, String url) {
                if (url != null && url.startsWith("https://" + APP_HOST)) {
                    return serveAsset(Uri.parse(url));
                }
                return null;
            }

            private boolean handleNavigation(Uri url) {
                if (url == null) return true;
                String host = url.getHost();
                String scheme = url.getScheme();
                if (APP_HOST.equals(host) || "file".equals(scheme)) {
                    return false; // in-app navigation stays in the WebView
                }
                if ("about".equals(scheme) || "data".equals(scheme)
                        || "blob".equals(scheme) || "javascript".equals(scheme)) {
                    return true; // never leave the app for these
                }
                // Real web links (LinkedIn / GitHub buttons) open in the browser.
                try {
                    Intent intent = new Intent(Intent.ACTION_VIEW, url);
                    intent.addCategory(Intent.CATEGORY_BROWSABLE);
                    startActivity(intent);
                } catch (Exception ignored) { }
                return true;
            }

            @Override
            public boolean shouldOverrideUrlLoading(WebView view,
                                                    WebResourceRequest request) {
                return handleNavigation(request.getUrl());
            }

            @Override
            @SuppressWarnings("deprecation")
            public boolean shouldOverrideUrlLoading(WebView view, String url) {
                return handleNavigation(url == null ? null : Uri.parse(url));
            }

            @Override
            public void onPageFinished(WebView view, String url) {
                super.onPageFinished(view, url);
                try {
                    view.evaluateJavascript(TTS_POLYFILL, null);
                } catch (Exception ignored) { }
            }

            @Override
            public void onReceivedError(WebView view, WebResourceRequest request,
                                        WebResourceError error) {
                super.onReceivedError(view, request, error);
                if (!request.isForMainFrame()) return;
                Uri url = request.getUrl();
                if (!fallbackTried && url != null && APP_HOST.equals(url.getHost())) {
                    // One automatic fallback: load the same app straight
                    // from the APK's assets.
                    fallbackTried = true;
                    try {
                        webView.getSettings().setAllowFileAccess(true);
                        webView.loadUrl(FALLBACK_URL);
                    } catch (Exception ignored) { }
                    return;
                }
                String desc = "";
                try {
                    desc = String.valueOf(error.getDescription())
                            + " (code " + error.getErrorCode() + ")";
                } catch (Exception ignored) { }
                showErrorPage("The app could not load.", desc);
            }
        });

        webView.loadUrl(HOME_URL);
    }

    @Override
    public void onBackPressed() {
        if (webView != null && webView.canGoBack()) {
            webView.goBack();
        } else {
            super.onBackPressed();
        }
    }

    @Override
    protected void onDestroy() {
        try {
            if (tts != null) {
                tts.stop();
                tts.shutdown();
                tts = null;
            }
        } catch (Exception ignored) { }
        try {
            if (webView != null) {
                webView.destroy();
                webView = null;
            }
        } catch (Exception ignored) { }
        super.onDestroy();
    }
}
