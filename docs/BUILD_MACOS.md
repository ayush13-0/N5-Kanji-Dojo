# macOS Build Guide

The handover records separate Apple Silicon and Intel ZIP packages. It does not contain enough source or build metadata to reproduce them.

Document the exact macOS SDK, Xcode / compiler, runtime, packaging commands, bundle identifier, entitlements and signing settings from the actual project.

## Release verification

- Confirm the Apple Silicon package contains arm64 code.
- Confirm the Intel package contains x86_64 code.
- Inspect the app bundle identifier and version.
- Verify code signatures.
- Check Gatekeeper assessment.
- State notarization status accurately; do not claim notarization unless the actual package is notarized.
- Test first launch, audio, persistence and exports on real Macs.
- Generate checksums for the final ZIP files.
