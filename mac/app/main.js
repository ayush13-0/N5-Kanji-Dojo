'use strict';
/*
 * N5 Kanji Dojo — macOS shell (Electron).
 * Security posture mirrors the Android/Windows shells: the study app runs
 * with no Node access, in a sandboxed renderer, from local files only.
 * Real web links (LinkedIn / GitHub) open in the default browser.
 * The app makes no network requests of its own; downloads the user starts
 * from the app's export page use the standard macOS save dialog.
 *
 * Audio (v1.5): speech inside the web view is not reliable, so — exactly
 * like the Windows shell — the page sends speak requests through the
 * chrome.webview-shaped bridge in preload.js and this process speaks them
 * with the Mac's built-in speech engine (/usr/bin/say). A Japanese voice
 * (Kyoko ships with macOS) is preferred; if none is installed, the kana is
 * transliterated to romaji and read with the system voice, so the audio
 * buttons are never silent, and Settings in the app can report which case
 * applies and offer the free Japanese-voice install in System Settings.
 */
const { app, BrowserWindow, shell, ipcMain } = require('electron');
const path = require('path');
const { pathToFileURL } = require('url');
const { execFile, spawn } = require('child_process');

const INDEX = path.join(__dirname, 'www', 'index.html');
const INDEX_URL = pathToFileURL(INDEX).toString();

app.setName('N5 Kanji Dojo');

function isAppUrl(url) {
  return typeof url === 'string' &&
    (url.startsWith(INDEX_URL) || url.startsWith('file://') ||
     url.startsWith('about:') || url.startsWith('blob:') ||
     url.startsWith('data:'));
}

/* ---------------- native speech (macOS `say`) ---------------- */

const SAY = '/usr/bin/say';
let voicesChecked = false;
let japaneseVoice = null;   // e.g. 'Kyoko'
let sayChild = null;

function detectVoices(done) {
  execFile(SAY, ['-v', '?'], { timeout: 8000 }, (err, stdout) => {
    voicesChecked = true;
    if (!err && stdout) {
      const ja = [];
      for (const line of stdout.split('\n')) {
        const m = line.match(/^(.+?)\s{2,}([a-z]{2}_[A-Z]{2})\b/);
        if (m && m[2].indexOf('ja_') === 0) ja.push(m[1].trim());
      }
      const pref = ['Kyoko', 'Otoya'];
      japaneseVoice = pref.find(n => ja.indexOf(n) >= 0) || ja[0] || null;
    }
    if (done) done();
  });
}

function ttsStatus() {
  return {
    type: 'tts-status',
    available: true, // /usr/bin/say is part of macOS itself
    japaneseVoice: !!japaneseVoice,
    voiceName: japaneseVoice || 'Mac system voice',
  };
}

function broadcastStatus() {
  for (const w of BrowserWindow.getAllWindows()) {
    if (!w.isDestroyed()) w.webContents.send('dojo-tts-status', ttsStatus());
  }
}

function stopSpeech() {
  if (sayChild) {
    try { sayChild.kill('SIGTERM'); } catch (_) {}
    sayChild = null;
  }
}

function speak(text, slow) {
  if (!text) return;
  stopSpeech();
  let args;
  if (japaneseVoice) {
    args = ['-v', japaneseVoice, '-r', slow ? '115' : '165', text];
  } else {
    const romaji = kanaToRomaji(text);
    args = ['-r', slow ? '120' : '175', romaji && romaji.trim() ? romaji : text];
  }
  try {
    sayChild = spawn(SAY, args, { stdio: 'ignore' });
    sayChild.on('error', () => { sayChild = null; });
    sayChild.on('exit', () => { sayChild = null; });
  } catch (_) { sayChild = null; }
}

/* ------- kana -> romaji (fallback only, mirrors the Windows shell) ------- */

const KANA_SINGLE = {
  'あ': 'a', 'い': 'i', 'う': 'u', 'え': 'e', 'お': 'o',
  'か': 'ka', 'き': 'ki', 'く': 'ku', 'け': 'ke', 'こ': 'ko',
  'さ': 'sa', 'し': 'shi', 'す': 'su', 'せ': 'se', 'そ': 'so',
  'た': 'ta', 'ち': 'chi', 'つ': 'tsu', 'て': 'te', 'と': 'to',
  'な': 'na', 'に': 'ni', 'ぬ': 'nu', 'ね': 'ne', 'の': 'no',
  'は': 'ha', 'ひ': 'hi', 'ふ': 'fu', 'へ': 'he', 'ほ': 'ho',
  'ま': 'ma', 'み': 'mi', 'む': 'mu', 'め': 'me', 'も': 'mo',
  'や': 'ya', 'ゆ': 'yu', 'よ': 'yo',
  'ら': 'ra', 'り': 'ri', 'る': 'ru', 'れ': 're', 'ろ': 'ro',
  'わ': 'wa', 'を': 'wo', 'ん': 'n',
  'が': 'ga', 'ぎ': 'gi', 'ぐ': 'gu', 'げ': 'ge', 'ご': 'go',
  'ざ': 'za', 'じ': 'ji', 'ず': 'zu', 'ぜ': 'ze', 'ぞ': 'zo',
  'だ': 'da', 'ぢ': 'ji', 'づ': 'zu', 'で': 'de', 'ど': 'do',
  'ば': 'ba', 'び': 'bi', 'ぶ': 'bu', 'べ': 'be', 'ぼ': 'bo',
  'ぱ': 'pa', 'ぴ': 'pi', 'ぷ': 'pu', 'ぺ': 'pe', 'ぽ': 'po',
  'ぁ': 'a', 'ぃ': 'i', 'ぅ': 'u', 'ぇ': 'e', 'ぉ': 'o',
  'ゃ': 'ya', 'ゅ': 'yu', 'ょ': 'yo', 'ゎ': 'wa',
};
const KANA_YOON = {
  'きゃ': 'kya', 'きゅ': 'kyu', 'きょ': 'kyo',
  'しゃ': 'sha', 'しゅ': 'shu', 'しょ': 'sho',
  'ちゃ': 'cha', 'ちゅ': 'chu', 'ちょ': 'cho',
  'にゃ': 'nya', 'にゅ': 'nyu', 'にょ': 'nyo',
  'ひゃ': 'hya', 'ひゅ': 'hyu', 'ひょ': 'hyo',
  'みゃ': 'mya', 'みゅ': 'myu', 'みょ': 'myo',
  'りゃ': 'rya', 'りゅ': 'ryu', 'りょ': 'ryo',
  'ぎゃ': 'gya', 'ぎゅ': 'gyu', 'ぎょ': 'gyo',
  'じゃ': 'ja', 'じゅ': 'ju', 'じょ': 'jo',
  'ぢゃ': 'ja', 'ぢゅ': 'ju', 'ぢょ': 'jo',
  'びゃ': 'bya', 'びゅ': 'byu', 'びょ': 'byo',
  'ぴゃ': 'pya', 'ぴゅ': 'pyu', 'ぴょ': 'pyo',
};
function toHiragana(c) {
  const code = c.codePointAt(0);
  if (code >= 0x30A1 && code <= 0x30F6) return String.fromCodePoint(code - 0x60);
  return c;
}
function romajiAt(s, i) {
  const c = toHiragana(s[i]);
  if (i + 1 < s.length) {
    const pair = c + toHiragana(s[i + 1]);
    if (KANA_YOON[pair]) return [KANA_YOON[pair], 2];
  }
  if (KANA_SINGLE[c]) return [KANA_SINGLE[c], 1];
  return ['', 0];
}
function kanaToRomaji(input) {
  let out = '';
  let last = '';
  for (let i = 0; i < input.length; i++) {
    const raw = input[i];
    if (raw === 'ー') {
      const m = /[aiueo]/.exec(last.split('').reverse().join(''));
      const vowel = m ? m[0] : '';
      if (vowel) { out += vowel; last += vowel; }
      continue;
    }
    const c = toHiragana(raw);
    if (c === 'っ') {
      if (i + 1 < input.length) {
        const [next, used] = romajiAt(input, i + 1);
        if (next && used) {
          const doubled = next.startsWith('ch') ? 't' + next : next[0] + next;
          out += doubled; last = doubled; i += used; continue;
        }
      }
      continue;
    }
    const [r, used] = romajiAt(input, i);
    if (r && used) { out += r; last = r; i += used - 1; }
    else { out += raw; last = ''; }
  }
  return out;
}

/* ---------------- page <-> shell messages ---------------- */

ipcMain.on('dojo-web-message', (event, raw) => {
  let msg = null;
  try { msg = JSON.parse(raw); } catch (_) { return; }
  if (!msg || typeof msg.type !== 'string') return;
  if (msg.type === 'speak') {
    speak(typeof msg.text === 'string' ? msg.text : '', !!msg.slow);
  } else if (msg.type === 'speak-stop') {
    stopSpeech();
  } else if (msg.type === 'tts-status-request') {
    const send = () => { if (!event.sender.isDestroyed()) event.sender.send('dojo-tts-status', ttsStatus()); };
    if (voicesChecked) send(); else detectVoices(send);
  } else if (msg.type === 'open-speech-settings') {
    const modern = 'x-apple.systempreferences:com.apple.Accessibility-Settings.extension';
    const legacy = 'x-apple.systempreferences:com.apple.preference.universalaccess';
    shell.openExternal(modern).catch(() => shell.openExternal(legacy).catch(() => {}));
  }
});

/* ---------------- window ---------------- */

function createWindow() {
  const win = new BrowserWindow({
    width: 1280,
    height: 820,
    minWidth: 380,
    minHeight: 480,
    title: 'N5 Kanji Dojo',
    webPreferences: {
      nodeIntegration: false,
      contextIsolation: true,
      sandbox: true,
      spellcheck: false,
      preload: path.join(__dirname, 'preload.js'),
    },
  });

  win.webContents.setWindowOpenHandler(({ url }) => {
    if (!isAppUrl(url)) shell.openExternal(url);
    return { action: 'deny' };
  });
  win.webContents.on('will-navigate', (event, url) => {
    if (!isAppUrl(url)) {
      event.preventDefault();
      shell.openExternal(url);
    }
  });
  win.webContents.on('did-finish-load', () => broadcastStatus());
  win.on('closed', stopSpeech);

  win.loadFile(INDEX);
  return win;
}

app.whenReady().then(() => {
  detectVoices(broadcastStatus);
  createWindow();
  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) createWindow();
  });
});

app.on('window-all-closed', () => {
  stopSpeech();
  if (process.platform !== 'darwin') app.quit();
});
app.on('before-quit', stopSpeech);
