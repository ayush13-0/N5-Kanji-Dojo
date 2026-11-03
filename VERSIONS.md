# Release and Version Matrix

This document records versions stated in the project handover supplied by the developer. It is a historical release register, not independent proof that each binary is currently available or authentic.

| Platform | Version | Package name recorded | Minimum / target details recorded |
|---|---:|---|---|
| Android | 1.6 | `N5-Kanji-Dojo-v1.6.apk` | Android 8+; target Android 16 / API 36 was stated for the v1.2 rebuild; confirm v1.6 manifest before publication |
| Windows | 1.6 | `N5-Kanji-Dojo-Setup-v1.6.exe` | Windows 11 setup; verify actual supported Windows versions on a clean machine |
| macOS Apple Silicon | 1.5 | `N5-Kanji-Dojo-v1.5-mac-AppleSilicon.zip` | Apple Silicon |
| macOS Intel | 1.5 | `N5-Kanji-Dojo-v1.5-mac-Intel.zip` | Intel Mac |

## Release history notes

- Android v1.6: reported as signed with the same signing key as earlier Android releases, zero permissions, offline operation, and signature / zip-alignment checks.
- Windows v1.6: reported to prefer installed Japanese female Windows voices such as Haruka, Ayumi or Nanami. The handover described signing with a developer-created certificate and timestamping.
- macOS v1.5: reported to use the macOS `say` speech engine and to be packaged separately for Apple Silicon and Intel.
- Earlier Windows v1.5 introduced native Windows speech-engine integration; v1.6 refined female-voice selection.
- Earlier Android releases progressed through v1.0, v1.2, v1.3 and v1.4 before v1.6.

## Checksums

Do not publish shortened historical hashes as verification values. Generate complete SHA-256 hashes from the exact files being uploaded:

```powershell
Get-FileHash .\N5-Kanji-Dojo-Setup-v1.6.exe -Algorithm SHA256
Get-FileHash .\N5-Kanji-Dojo-v1.6.apk -Algorithm SHA256
```

```bash
shasum -a 256 N5-Kanji-Dojo-v1.5-mac-AppleSilicon.zip
shasum -a 256 N5-Kanji-Dojo-v1.5-mac-Intel.zip
```

Record the complete outputs in the GitHub Release description and `checksums-sha256.txt`. Never copy a checksum from a different build.

## Signing and verification

- Android: verify package identity, signing certificate, v1/v2/v3 signatures as applicable, and alignment using Android SDK tools.
- Windows: verify Authenticode signature and timestamp with `Get-AuthenticodeSignature` and SignTool. A self-issued certificate is not equivalent to a publicly trusted CA-issued certificate.
- macOS: verify architecture, bundle identifier, code signature and Gatekeeper assessment. Do not claim notarization unless it has actually completed successfully.

See [SECURITY.md](SECURITY.md) and [RELEASE_PROCESS.md](docs/RELEASE_PROCESS.md).
