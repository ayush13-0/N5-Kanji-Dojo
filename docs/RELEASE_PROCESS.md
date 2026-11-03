# Release Process

## 1. Prepare

- Confirm the source commit and release version.
- Update `VERSIONS.md` and `CHANGELOG.md`.
- Review documentation and third-party notices.
- Ensure no credentials, private keys or personal data are staged.
- Build each platform from the intended source revision.

## 2. Test

- Install each artifact on a clean supported device / OS.
- Test first launch, all quiz modes, search, custom vocabulary, progress persistence and erase option.
- Test audio and Japanese voice selection.
- Test CSV export.
- Test offline behaviour.
- Test upgrade and uninstall behaviour.
- Record failures and supported configurations.

## 3. Verify artifacts

Windows PowerShell:

```powershell
Get-FileHash .\N5-Kanji-Dojo-Setup-v1.6.exe -Algorithm SHA256
Get-AuthenticodeSignature .\N5-Kanji-Dojo-Setup-v1.6.exe | Format-List
```

Android (Android SDK build-tools):

```bash
apksigner verify --verbose --print-certs N5-Kanji-Dojo-v1.6.apk
zipalign -c -v 4 N5-Kanji-Dojo-v1.6.apk
```

macOS:

```bash
shasum -a 256 N5-Kanji-Dojo-v1.5-mac-AppleSilicon.zip
shasum -a 256 N5-Kanji-Dojo-v1.5-mac-Intel.zip
```

Record full hashes in `checksums-sha256.txt`. Verify signatures using the appropriate platform tools.

## 4. Publish GitHub Release

1. Push source and documentation to the repository.
2. Create a tag, for example `v1.6`.
3. Create a GitHub Release from that tag.
4. Upload the actual platform packages.
5. Upload the demo video if redistribution is permitted and file size allows.
6. Paste full SHA-256 values and installation notes.
7. Confirm every asset downloads and matches its published checksum.

## 5. After publishing

- Test download links while logged out.
- Confirm release notes and asset names.
- Keep a secure copy of signing material.
- Record any hotfix as a new version; do not silently replace a published binary.
