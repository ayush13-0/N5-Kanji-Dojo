'use strict';
/*
 * N5 Kanji Dojo — Mac preload bridge.
 * The study page already knows how to talk to a native speech bridge
 * through window.chrome.webview (the protocol built for the Windows app:
 * JSON messages out, 'message' events back). This preload exposes exactly
 * that shape — and nothing else — so the page's audio runs on the Mac's
 * native speech engine in the main process. No Node access reaches the
 * page: it can only send those JSON strings and receive TTS status.
 */
const { contextBridge, ipcRenderer } = require('electron');

const listeners = [];

ipcRenderer.on('dojo-tts-status', (_event, status) => {
  const data = JSON.stringify(status);
  for (const fn of listeners.slice()) {
    try { fn({ data }); } catch (_) { /* page listener errors stay in the page */ }
  }
});

contextBridge.exposeInMainWorld('chrome', {
  webview: {
    postMessage(message) {
      try {
        ipcRenderer.send('dojo-web-message',
          typeof message === 'string' ? message : JSON.stringify(message));
      } catch (_) { /* bridge unavailable: page falls back gracefully */ }
    },
    addEventListener(type, fn) {
      if (type === 'message' && typeof fn === 'function') listeners.push(fn);
    },
    removeEventListener(type, fn) {
      const i = listeners.indexOf(fn);
      if (i >= 0) listeners.splice(i, 1);
    },
  },
});
