# N5 Kanji Dojo

**A cross-platform Japanese JLPT N5 study application by Ayush Kumar.**

N5 Kanji Dojo is a Japanese-learning project designed to help learners practise JLPT N5 kanji, vocabulary, kana, pronunciation and recall through structured lessons and interactive quiz modes.

<p align="center">
  <strong>Android · Windows · macOS</strong><br>
  <em>Learn kanji. Build vocabulary. Practise every day.</em>
</p>

---

## Project status

| Platform | Release recorded in project history | Package |
|---|---:|---|
| Android | v1.6 | APK |
| Windows | v1.6 | Setup executable |
| macOS Apple Silicon | v1.5 | ZIP |
| macOS Intel | v1.5 | ZIP |

> **Release integrity notice:** These version numbers and feature descriptions are transcribed from the project handover notes. The binary packages are not included in this source repository package. Before publishing a GitHub Release, upload the actual installer files and generate fresh SHA-256 checksums using the instructions in [`docs/RELEASE_PROCESS.md`](docs/RELEASE_PROCESS.md). Do not treat historical abbreviated hashes as full checksums.

## Highlights

According to the supplied project handover:

- 15 quiz modes, including Vocabulary Course and Fill the Missing Kanji.
- Add-your-own vocabulary, saved locally on the device.
- Learning progress, settings and quiz history saved between sessions.
- Search, kanji pages, quizzes and CSV export support for user-added vocabulary.
- Compact study interface with Mobile / Desktop layout options.
- Japanese text-to-speech support through the host operating system.
- Offline-first design; the Android v1.6 package was reported as having zero Android permissions.
- Personal attribution and links to the developer.

Feature availability can differ by platform and release. See [`VERSIONS.md`](VERSIONS.md).

## Downloads

Installers should be published under **GitHub Releases**, not committed to the source tree.

| Platform | Release asset expected |
|---|---|
| Android | `N5-Kanji-Dojo-v1.6.apk` |
| Windows | `N5-Kanji-Dojo-Setup-v1.6.exe` |
| macOS Apple Silicon | `N5-Kanji-Dojo-v1.5-mac-AppleSilicon.zip` |
| macOS Intel | `N5-Kanji-Dojo-v1.5-mac-Intel.zip` |

When uploaded, visit the repository's **Releases** page and select the appropriate version. Until the assets are uploaded, these are expected filenames, not active download links.

## Installation

Platform-specific instructions:

- [Android installation](docs/INSTALL_ANDROID.md)
- [Windows installation](docs/INSTALL_WINDOWS.md)
- [macOS installation](docs/INSTALL_MACOS.md)
- [Japanese speech and audio setup](docs/AUDIO_SETUP.md)

## Demonstration

Place the demonstration video in `demo/Demo-Kanji-Dojo.mp4` before publishing, if you have permission to distribute it. The original local path supplied by the developer was:

`C:\Users\lenovo\Videos\AYUSH BIN\Demo - Kanji Dojo.mp4`

That local file is not included in this repository package. See [`demo/README.md`](demo/README.md).

## Documentation

| Document | Purpose |
|---|---|
| [VERSIONS.md](VERSIONS.md) | Platform release matrix |
| [CHANGELOG.md](CHANGELOG.md) | Version history |
| [ARCHITECTURE.md](docs/ARCHITECTURE.md) | High-level architecture notes |
| [SECURITY.md](SECURITY.md) | Security policy |
| [PRIVACY.md](PRIVACY.md) | Privacy statement and verification boundaries |
| [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md) | Third-party attribution register |
| [BUILD_ANDROID.md](docs/BUILD_ANDROID.md) | Android build notes |
| [BUILD_WINDOWS.md](docs/BUILD_WINDOWS.md) | Windows build notes |
| [BUILD_MACOS.md](docs/BUILD_MACOS.md) | macOS build notes |
| [RELEASE_PROCESS.md](docs/RELEASE_PROCESS.md) | Packaging and release checklist |
| [FAQ.md](docs/FAQ.md) | Troubleshooting and common questions |

## Developer

**Built by Ayush Kumar**

- GitHub: [@ayush13-0](https://github.com/ayush13-0)
- LinkedIn: [Ayush Kumar](https://www.linkedin.com/)

> Replace the LinkedIn URL above with the exact public profile URL before publishing if it differs. The supplied handover refers to an exact LinkedIn URL but does not contain it.

## Repository layout

```text
N5-Kanji-Dojo/
├── .github/                 # Issue and pull-request templates
├── app/                     # Application source (add actual source)
├── assets/                  # Project-owned images and resources
├── demo/                    # Demonstration video
├── docs/                    # Guides and technical documentation
├── licenses/                # Third-party licence texts
├── scripts/                 # Release helper scripts
├── src/                     # Platform shell/source (add actual source)
├── .gitignore
├── CHANGELOG.md
├── CONTRIBUTING.md
├── CODE_OF_CONDUCT.md
├── LICENSE
├── PRIVACY.md
├── README.md
├── SECURITY.md
├── THIRD_PARTY_NOTICES.md
└── VERSIONS.md
```

The `app/` and `src/` folders in this documentation package are placeholders. Add the real application source before describing this repository as a source-code release.

## Licence

See [`LICENSE`](LICENSE). The current file reserves rights; it does not grant permission to redistribute or modify the application. Confirm ownership and third-party licence obligations before changing the licence.
