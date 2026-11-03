# Japanese Audio Setup

## Platform behaviour recorded in the handover

| Platform | Speech route described |
|---|---|
| Android | Device text-to-speech engine |
| Windows | Native Windows speech engine |
| macOS | Native `say` speech engine |

The Windows v1.6 handover says the app prioritises Japanese female voices such as Haruka, Ayumi and Nanami. Actual voice availability depends on installed system speech packages and OS version.

## Windows

1. Open Windows Settings.
2. Find Time & language → Speech or the relevant Text-to-speech / language settings.
3. Add Japanese speech resources, including Text-to-speech.
4. Wait for installation to finish.
5. Restart N5 Kanji Dojo.
6. Open Settings → Audio on this device and use the test phrase.

Names and navigation can vary by Windows build.

## macOS

Check the system Spoken Content / Accessibility speech settings and install a Japanese voice if required. The app handover describes use of `say`.

## Android

Open the device's Text-to-speech output settings and install/select a Japanese voice.

## Troubleshooting

- Confirm system volume and output device.
- Test Japanese speech in the operating system outside the app.
- Restart the app after installing a new voice.
- Confirm the selected voice supports Japanese.
- Record the OS version, app version and exact error for a bug report.

A fallback voice may pronounce romaji rather than Japanese. That is not equivalent to native Japanese pronunciation.
