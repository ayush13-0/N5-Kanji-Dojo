# Windows Build Guide

The project handover describes a Windows installer and native Windows speech integration. It does not provide enough build-system detail to reproduce the installer from scratch.

Before documenting a reproducible build, add:

- Exact source tree and runtime versions.
- Compiler / packaging tool versions.
- Installer authoring configuration.
- WebView2 or other runtime prerequisites, if applicable.
- Code-signing certificate handling and timestamp service.
- Exact build commands.

## Release verification

- Test install, first launch, upgrade and uninstall on a clean supported Windows machine.
- Test Japanese female voice selection and fallback behaviour.
- Inspect bundled files and permissions.
- Verify Authenticode signature and timestamp.
- Calculate SHA-256 after the final signed installer is produced.
- Keep the signing certificate's private key and password out of Git.
