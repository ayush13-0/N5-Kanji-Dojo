using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace N5KanjiDojo
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    /// <summary>
    /// Native Windows text-to-speech through the built-in Windows speech
    /// engine (SAPI, "SAPI.SpVoice"). WebView2's web speechSynthesis API
    /// exposes no working voices, so the page sends speak requests to the
    /// shell with chrome.webview.postMessage and this class speaks them.
    /// Prefers an installed Japanese voice; when none exists it reads the
    /// romaji transliteration of the kana aloud with the default voice so
    /// the audio buttons are never silent, and reports that state back to
    /// the page so Settings can offer the free Japanese-voice install.
    /// All methods are defensive: an audio failure must never crash the app.
    /// </summary>
    internal sealed class WindowsTts
    {
        private const int SpeakAsyncPurge = 3; // SVSFlagsAsync(1) | SVSFPurgeBeforeSpeak(2)
        private dynamic _voice;                // SAPI.SpVoice (COM, late bound)
        private bool _initTried;
        public bool Available { get; private set; }
        public bool HasJapaneseVoice { get; private set; }
        public string VoiceName { get; private set; } = "";

        private void EnsureInit()
        {
            if (_initTried) return;
            _initTried = true;
            try
            {
                var t = Type.GetTypeFromProgID("SAPI.SpVoice");
                if (t == null) return;
                _voice = Activator.CreateInstance(t);
                _voice.Volume = 100;

                dynamic japanese = null;
                string japaneseName = "";
                int japaneseScore = int.MaxValue;
                dynamic first = null;
                string firstName = "";
                try
                {
                    dynamic voices = _voice.GetVoices("", "");
                    int count = 0;
                    try { count = voices.Count; } catch { }
                    for (int i = 0; i < count; i++)
                    {
                        dynamic token = null;
                        string desc = "", lang = "";
                        try { token = voices.Item(i); } catch { }
                        if (token == null) continue;
                        try { desc = token.GetDescription(0) ?? ""; } catch { }
                        try { lang = token.GetAttribute("Language") ?? ""; } catch { }
                        if (first == null) { first = token; firstName = desc; }
                        bool isJa = lang.StartsWith("411", StringComparison.OrdinalIgnoreCase)
                            || desc.IndexOf("Japan", StringComparison.OrdinalIgnoreCase) >= 0
                            || desc.IndexOf("Haruka", StringComparison.OrdinalIgnoreCase) >= 0
                            || desc.IndexOf("Ayumi", StringComparison.OrdinalIgnoreCase) >= 0
                            || desc.IndexOf("Ichiro", StringComparison.OrdinalIgnoreCase) >= 0
                            || desc.IndexOf("Sayaka", StringComparison.OrdinalIgnoreCase) >= 0
                            || desc.IndexOf("Nanami", StringComparison.OrdinalIgnoreCase) >= 0;
                        if (isJa)
                        {
                            // The user wants the Japanese *female* voice, like
                            // the Android app's voice — score candidates so a
                            // female Japanese voice always wins over a male one.
                            int score = JapaneseVoiceScore(desc);
                            if (score < japaneseScore)
                            {
                                japaneseScore = score;
                                japanese = token;
                                japaneseName = desc;
                            }
                        }
                    }
                }
                catch { /* voice enumeration is best-effort */ }

                if (japanese != null)
                {
                    try { _voice.Voice = japanese; } catch { }
                    HasJapaneseVoice = true;
                    VoiceName = japaneseName;
                }
                else if (first != null)
                {
                    VoiceName = firstName;
                }
                Available = true;
            }
            catch
            {
                Available = false;
                _voice = null;
            }
        }

        // Lower is preferred. Known Japanese female voices first (Nanami is
        // the natural female voice, Haruka/Ayumi the classic female voices),
        // then any other Japanese voice, then known male voices (Ichiro…).
        private static int JapaneseVoiceScore(string desc)
        {
            string[] female = { "Nanami", "Haruka", "Ayumi", "Sayaka", "Kyoko" };
            for (int i = 0; i < female.Length; i++)
                if (desc.IndexOf(female[i], StringComparison.OrdinalIgnoreCase) >= 0) return i;
            string[] male = { "Ichiro", "Kenta", "Naoki", "Daichi" };
            foreach (string m in male)
                if (desc.IndexOf(m, StringComparison.OrdinalIgnoreCase) >= 0) return 20;
            return 10;
        }

        public void Speak(string text, bool slow)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            EnsureInit();
            if (!Available || _voice == null) return;
            try
            {
                if (HasJapaneseVoice)
                {
                    _voice.Rate = slow ? -4 : -1;
                    _voice.Speak(text, SpeakAsyncPurge);
                }
                else
                {
                    string romaji = KanaToRomaji.Convert(text);
                    _voice.Rate = slow ? -3 : 0;
                    _voice.Speak(string.IsNullOrWhiteSpace(romaji) ? text : romaji, SpeakAsyncPurge);
                }
            }
            catch { /* never let audio break the app */ }
        }

        public void Stop()
        {
            if (!Available || _voice == null) return;
            try { _voice.Speak("", 2); } catch { }
        }

        public string StatusJson()
        {
            EnsureInit();
            using (var ms = new MemoryStream())
            {
                using (var w = new Utf8JsonWriter(ms))
                {
                    w.WriteStartObject();
                    w.WriteString("type", "tts-status");
                    w.WriteBoolean("available", Available);
                    w.WriteBoolean("japaneseVoice", HasJapaneseVoice);
                    w.WriteString("voiceName", VoiceName ?? "");
                    w.WriteEndObject();
                }
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }

    /// <summary>
    /// Hiragana/katakana to romaji (Hepburn-style), used only as the audible
    /// fallback when the PC has no Japanese TTS voice installed: an English
    /// voice cannot pronounce kana characters at all, but it can read the
    /// romaji spelling, so learners still hear an approximation.
    /// Non-kana characters are passed through unchanged.
    /// </summary>
    internal static class KanaToRomaji
    {
        private static readonly Dictionary<char, string> Single = new Dictionary<char, string>
        {
            {'あ',"a"},{'い',"i"},{'う',"u"},{'え',"e"},{'お',"o"},
            {'か',"ka"},{'き',"ki"},{'く',"ku"},{'け',"ke"},{'こ',"ko"},
            {'さ',"sa"},{'し',"shi"},{'す',"su"},{'せ',"se"},{'そ',"so"},
            {'た',"ta"},{'ち',"chi"},{'つ',"tsu"},{'て',"te"},{'と',"to"},
            {'な',"na"},{'に',"ni"},{'ぬ',"nu"},{'ね',"ne"},{'の',"no"},
            {'は',"ha"},{'ひ',"hi"},{'ふ',"fu"},{'へ',"he"},{'ほ',"ho"},
            {'ま',"ma"},{'み',"mi"},{'む',"mu"},{'め',"me"},{'も',"mo"},
            {'や',"ya"},{'ゆ',"yu"},{'よ',"yo"},
            {'ら',"ra"},{'り',"ri"},{'る',"ru"},{'れ',"re"},{'ろ',"ro"},
            {'わ',"wa"},{'を',"wo"},{'ん',"n"},
            {'が',"ga"},{'ぎ',"gi"},{'ぐ',"gu"},{'げ',"ge"},{'ご',"go"},
            {'ざ',"za"},{'じ',"ji"},{'ず',"zu"},{'ぜ',"ze"},{'ぞ',"zo"},
            {'だ',"da"},{'ぢ',"ji"},{'づ',"zu"},{'で',"de"},{'ど',"do"},
            {'ば',"ba"},{'び',"bi"},{'ぶ',"bu"},{'べ',"be"},{'ぼ',"bo"},
            {'ぱ',"pa"},{'ぴ',"pi"},{'ぷ',"pu"},{'ぺ',"pe"},{'ぽ',"po"},
            {'ぁ',"a"},{'ぃ',"i"},{'ぅ',"u"},{'ぇ',"e"},{'ぉ',"o"},
            {'ゃ',"ya"},{'ゅ',"yu"},{'ょ',"yo"},{'ゎ',"wa"},
        };

        private static readonly Dictionary<string, string> Yoon = new Dictionary<string, string>
        {
            {"きゃ","kya"},{"きゅ","kyu"},{"きょ","kyo"},
            {"しゃ","sha"},{"しゅ","shu"},{"しょ","sho"},
            {"ちゃ","cha"},{"ちゅ","chu"},{"ちょ","cho"},
            {"にゃ","nya"},{"にゅ","nyu"},{"にょ","nyo"},
            {"ひゃ","hya"},{"ひゅ","hyu"},{"ひょ","hyo"},
            {"みゃ","mya"},{"みゅ","myu"},{"みょ","myo"},
            {"りゃ","rya"},{"りゅ","ryu"},{"りょ","ryo"},
            {"ぎゃ","gya"},{"ぎゅ","gyu"},{"ぎょ","gyo"},
            {"じゃ","ja"},{"じゅ","ju"},{"じょ","jo"},
            {"ぢゃ","ja"},{"ぢゅ","ju"},{"ぢょ","jo"},
            {"びゃ","bya"},{"びゅ","byu"},{"びょ","byo"},
            {"ぴゃ","pya"},{"ぴゅ","pyu"},{"ぴょ","pyo"},
        };

        private static char KatakanaToHiragana(char c)
        {
            if (c >= 'ァ' && c <= 'ヶ') return (char)(c - 0x60);
            if (c == 'ヵ') return 'か';
            if (c == 'ヶ') return 'け';
            return c;
        }

        private static bool IsSmallY(char c) { return c == 'ゃ' || c == 'ゅ' || c == 'ょ'; }

        private static string LastVowel(string s)
        {
            for (int i = s.Length - 1; i >= 0; i--)
            {
                char c = s[i];
                if (c == 'a' || c == 'i' || c == 'u' || c == 'e' || c == 'o') return c.ToString();
            }
            return "";
        }

        public static string Convert(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var sb = new StringBuilder();
            string lastRomaji = "";
            for (int i = 0; i < input.Length; i++)
            {
                char raw = input[i];
                if (raw == 'ー') // long-vowel mark: repeat the previous vowel
                {
                    string v = LastVowel(lastRomaji);
                    if (v.Length > 0) { sb.Append(v); lastRomaji += v; }
                    continue;
                }
                char c = KatakanaToHiragana(raw);
                if (c == 'っ') // sokuon: double the next consonant
                {
                    if (i + 1 < input.Length)
                    {
                        string next = RomajiAt(input, i + 1, out int used);
                        if (next.Length > 0 && used > 0)
                        {
                            string doubled = next.StartsWith("ch", StringComparison.Ordinal)
                                ? "t" + next
                                : next.Substring(0, 1) + next;
                            sb.Append(doubled);
                            lastRomaji = doubled;
                            i += used;
                            continue;
                        }
                    }
                    continue;
                }
                string r = RomajiAt(input, i, out int consumed);
                if (r.Length > 0 && consumed > 0)
                {
                    sb.Append(r);
                    lastRomaji = r;
                    i += consumed - 1;
                }
                else
                {
                    sb.Append(raw);
                    lastRomaji = "";
                }
            }
            return sb.ToString();
        }

        private static string RomajiAt(string s, int i, out int consumed)
        {
            consumed = 0;
            if (i >= s.Length) return "";
            char c = KatakanaToHiragana(s[i]);
            if (i + 1 < s.Length)
            {
                char n = KatakanaToHiragana(s[i + 1]);
                if (IsSmallY(n))
                {
                    string pair = c.ToString() + n.ToString();
                    if (Yoon.TryGetValue(pair, out string y)) { consumed = 2; return y; }
                }
            }
            if (Single.TryGetValue(c, out string r)) { consumed = 1; return r; }
            return "";
        }
    }

    internal sealed class MainForm : Form
    {
        private const string HostName = "dojo.app";
        private readonly WebView2 _web = new WebView2();
        private readonly Label _status = new Label();
        private readonly WindowsTts _tts = new WindowsTts();

        public MainForm()
        {
            Text = "N5 Kanji Dojo";
            Width = 1280;
            Height = 820;
            MinimumSize = new System.Drawing.Size(380, 480);
            StartPosition = FormStartPosition.CenterScreen;
            try
            {
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(
                    Application.ExecutablePath);
            }
            catch { /* icon is cosmetic */ }

            _status.Dock = DockStyle.Fill;
            _status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            _status.Text = "Loading N5 Kanji Dojo…";
            _status.Font = new System.Drawing.Font("Segoe UI", 12f);
            Controls.Add(_status);

            _web.Dock = DockStyle.Fill;
            _web.Visible = false;
            Controls.Add(_web);
            _web.BringToFront();

            Shown += async (s, e) => await InitAsync();
        }

        private static string AppDataDir()
        {
            string dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "N5KanjiDojo");
            Directory.CreateDirectory(dir);
            return dir;
        }

        private static string ExtractPayload(string root)
        {
            string www = Path.Combine(root, "www");
            Directory.CreateDirectory(www);
            WriteResourceIfChanged("index.html", Path.Combine(www, "index.html"));
            WriteResourceIfChanged("licenses.txt", Path.Combine(root, "licenses.txt"));
            return www;
        }

        private static void WriteResourceIfChanged(string logicalName, string dest)
        {
            using (var stream = Assembly.GetExecutingAssembly()
                       .GetManifestResourceStream(logicalName))
            {
                if (stream == null) return;
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    byte[] fresh = ms.ToArray();
                    bool same = false;
                    if (File.Exists(dest))
                    {
                        using (var sha = SHA256.Create())
                        {
                            byte[] oldHash = sha.ComputeHash(File.ReadAllBytes(dest));
                            byte[] newHash = sha.ComputeHash(fresh);
                            same = Convert.ToBase64String(oldHash) ==
                                   Convert.ToBase64String(newHash);
                        }
                    }
                    if (!same) File.WriteAllBytes(dest, fresh);
                }
            }
        }

        private async Task InitAsync()
        {
            try
            {
                string root = AppDataDir();
                string www = ExtractPayload(root);
                string userData = Path.Combine(root, "WebView2Data");
                Directory.CreateDirectory(userData);

                var env = await CoreWebView2Environment.CreateAsync(null, userData);
                await _web.EnsureCoreWebView2Async(env);
                var core = _web.CoreWebView2;

                var s = core.Settings;
                s.IsScriptEnabled = true;
                s.IsWebMessageEnabled = true;
                s.AreDevToolsEnabled = false;
                s.AreBrowserAcceleratorKeysEnabled = false;
                s.IsStatusBarEnabled = false;
                s.IsZoomControlEnabled = true;
                s.IsGeneralAutofillEnabled = false;
                s.IsPasswordAutosaveEnabled = false;

                core.SetVirtualHostNameToFolderMapping(
                    HostName, www, CoreWebView2HostResourceAccessKind.Allow);

                core.WebMessageReceived += OnWebMessage;
                core.NavigationStarting += (sender, args) =>
                {
                    if (!IsAppUrl(args.Uri))
                    {
                        args.Cancel = true;
                        OpenExternal(args.Uri);
                    }
                };
                core.NewWindowRequested += (sender, args) =>
                {
                    args.Handled = true;
                    if (!IsAppUrl(args.Uri)) OpenExternal(args.Uri);
                };
                core.NavigationCompleted += (sender, args) =>
                {
                    if (args.IsSuccess)
                    {
                        _status.Visible = false;
                        _web.Visible = true;
                        SendTtsStatus();
                    }
                };

                core.Navigate("https://" + HostName + "/index.html");
            }
            catch (Exception ex)
            {
                _status.Text = "N5 Kanji Dojo could not start.";
                MessageBox.Show(
                    "N5 Kanji Dojo needs the Microsoft Edge WebView2 Runtime, " +
                    "which is built into Windows 11 and most Windows 10 PCs.\n\n" +
                    "If it is missing, install the free 'Microsoft Edge WebView2 Runtime' " +
                    "from Microsoft's website, then start the app again.\n\n" +
                    "Details: " + ex.Message,
                    "N5 Kanji Dojo — could not start",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SendTtsStatus()
        {
            try { _web.CoreWebView2.PostWebMessageAsJson(_tts.StatusJson()); }
            catch { /* page also requests status itself */ }
        }

        private void OnWebMessage(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            string raw = null;
            try { raw = args.TryGetWebMessageAsString(); }
            catch { }
            if (string.IsNullOrEmpty(raw))
            {
                try { raw = args.WebMessageAsJson; } catch { }
            }
            if (string.IsNullOrEmpty(raw)) return;

            try
            {
                using (var doc = JsonDocument.Parse(raw))
                {
                    var r = doc.RootElement;
                    string type = r.TryGetProperty("type", out var tp) ? tp.GetString() : null;
                    if (type == "speak")
                    {
                        string text = r.TryGetProperty("text", out var tx) ? tx.GetString() : null;
                        bool slow = r.TryGetProperty("slow", out var sl) && sl.ValueKind == JsonValueKind.True;
                        _tts.Speak(text, slow);
                    }
                    else if (type == "speak-stop")
                    {
                        _tts.Stop();
                    }
                    else if (type == "tts-status-request")
                    {
                        SendTtsStatus();
                    }
                    else if (type == "open-speech-settings")
                    {
                        OpenExternal("ms-settings:regionlanguage");
                    }
                }
            }
            catch { /* malformed page messages are ignored */ }
        }

        private static bool IsAppUrl(string uri)
        {
            if (string.IsNullOrEmpty(uri)) return true;
            if (uri.StartsWith("about:", StringComparison.OrdinalIgnoreCase)) return true;
            if (uri.StartsWith("blob:", StringComparison.OrdinalIgnoreCase)) return true;
            if (uri.StartsWith("data:", StringComparison.OrdinalIgnoreCase)) return true;
            Uri u;
            return Uri.TryCreate(uri, UriKind.Absolute, out u) && u.Host == HostName;
        }

        private static void OpenExternal(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri)) return;
            try
            {
                Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
            }
            catch { /* nothing sensible to do if no handler exists */ }
        }
    }
}
