# Android Build Guide

This is a release checklist, not a complete reproducible build recipe. Add the exact Gradle / Android SDK versions and commands from the actual project source before claiming reproducibility.

## Required information

- Android application source and package identifier.
- Exact JDK version.
- Android Gradle Plugin and Gradle wrapper versions.
- Compile SDK, target SDK and minimum SDK.
- Signing configuration supplied securely outside source control.

## Build and verify

1. Build a release APK from the real source.
2. Inspect the merged manifest and permissions.
3. Verify the package identifier and version code.
4. Verify signing certificate and signature schemes.
5. Run `zipalign -c -v 4 app-release.apk`.
6. Install on supported Android test devices.
7. Test offline behaviour, audio, persistence, export and upgrade.
8. Generate a SHA-256 checksum.

Never commit signing passwords or private keystores.
