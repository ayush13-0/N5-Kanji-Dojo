<div align="center">

<img src="https://capsule-render.vercel.app/api?type=rect&color=0:141E30,100:243B55&height=140&section=header&text=N5%20Kanji%20Dojo&fontSize=44&fontColor=ffffff&desc=漢字道場%20·%20JLPT%20N5%20Learning%20Application&descSize=18&descAlignY=72&fontAlignY=42" width="100%" alt="N5 Kanji Dojo">

<br>

**A cross-platform application for JLPT N5 kanji, vocabulary, and pronunciation practice.**

<br>

<p>
  <img src="https://img.shields.io/badge/version-1.6-243B55?style=flat-square" alt="Version">
  <img src="https://img.shields.io/badge/JLPT-N5-243B55?style=flat-square" alt="JLPT N5">
  <img src="https://img.shields.io/badge/platforms-Android%20%7C%20Windows%20%7C%20macOS-243B55?style=flat-square" alt="Platforms">
  <img src="https://img.shields.io/badge/design-offline--first-243B55?style=flat-square" alt="Offline first">
  <img src="https://img.shields.io/badge/license-all%20rights%20reserved-243B55?style=flat-square" alt="License">
</p>

<p>
  <img src="https://img.shields.io/github/v/release/ayush13-0/N5-Kanji-Dojo?style=flat-square&color=8E44AD" alt="Latest release">
  <img src="https://img.shields.io/github/downloads/ayush13-0/N5-Kanji-Dojo/total?style=flat-square&color=8E44AD" alt="Downloads">
  <img src="https://img.shields.io/github/last-commit/ayush13-0/N5-Kanji-Dojo?style=flat-square&color=8E44AD" alt="Last commit">
  <img src="https://img.shields.io/github/issues/ayush13-0/N5-Kanji-Dojo?style=flat-square&color=8E44AD" alt="Issues">
  <img src="https://img.shields.io/github/stars/ayush13-0/N5-Kanji-Dojo?style=flat-square&color=8E44AD" alt="Stars">
</p>

<p>
  <a href="#overview">Overview</a> &nbsp;·&nbsp;
  <a href="#features">Features</a> &nbsp;·&nbsp;
  <a href="#platform-support">Platforms</a> &nbsp;·&nbsp;
  <a href="#installation">Installation</a> &nbsp;·&nbsp;
  <a href="#repository-structure">Structure</a> &nbsp;·&nbsp;
  <a href="#documentation">Documentation</a> &nbsp;·&nbsp;
  <a href="#contributing">Contributing</a>
</p>

</div>

---

## Overview

N5 Kanji Dojo is a cross-platform Japanese learning application developed by **Ayush Kumar** to support preparation for the **Japanese Language Proficiency Test (JLPT) N5**.

It brings structured learning, interactive quizzes, kanji recognition, vocabulary practice, pronunciation support, and progress tracking together in a single study environment designed for consistent daily practice.

<div align="center">

| JLPT N5 | 15 | 4 | Offline |
|:---:|:---:|:---:|:---:|
| Focused syllabus | Reported quiz modes | Platform builds | First design |

</div>

<sub>Figures are drawn from the project handover notes.</sub>

### Study Workflow

<div align="center">

```mermaid
flowchart LR
    A["Learn<br>Kanji and vocabulary"] --> B["Practise<br>Quiz modes"]
    B --> C["Listen<br>Pronunciation"]
    C --> D["Review<br>Progress and history"]
    D --> A

    classDef step fill:#243B55,stroke:#8E44AD,color:#ffffff,stroke-width:1.5px;
    class A,B,C,D step;
```

</div>

---

## Features

<table>
<tr>
<td width="50%" valign="top">

**Japanese Learning**
- JLPT N5 kanji practice
- Vocabulary learning
- Kana practice
- Kanji recognition
- Japanese pronunciation

</td>
<td width="50%" valign="top">

**Interactive Practice**
- 15 reported quiz modes
- Vocabulary Course
- Fill the Missing Kanji
- Active recall exercises
- Knowledge reinforcement

</td>
</tr>
<tr>
<td width="50%" valign="top">

**Learning Management**
- Learning progress tracking
- Quiz history
- Persistent settings
- Locally saved custom vocabulary
- CSV vocabulary export

</td>
<td width="50%" valign="top">

**Application Experience**
- Mobile and desktop layouts
- Search functionality
- Individual kanji pages
- Operating-system text-to-speech
- Offline-first design

</td>
</tr>
</table>

<sub>Features are transcribed from the project handover. Availability may vary between platform versions.</sub>

---

## Screenshots

<!-- Add screenshots to assets/screenshots/ using the filenames below. -->

<table>
<tr>
<td align="center" width="50%">
<img src="assets/screenshots/home.png" width="100%" alt="Home screen"><br>
<sub><b>Home</b></sub>
</td>
<td align="center" width="50%">
<img src="assets/screenshots/learning.png" width="100%" alt="Learning interface"><br>
<sub><b>Learning</b></sub>
</td>
</tr>
<tr>
<td align="center" width="50%">
<img src="assets/screenshots/kanji.png" width="100%" alt="Kanji practice"><br>
<sub><b>Kanji Practice</b></sub>
</td>
<td align="center" width="50%">
<img src="assets/screenshots/quiz.png" width="100%" alt="Quiz interface"><br>
<sub><b>Quiz</b></sub>
</td>
</tr>
</table>

> Screenshots will display once the image files are added to `assets/screenshots/`.

---

## Platform Support

<div align="center">

| Platform | Version | Distribution |
|:---|:---:|:---:|
| Android | 1.6 | APK |
| Windows | 1.6 | Setup EXE |
| macOS (Apple Silicon) | 1.5 | ZIP |
| macOS (Intel) | 1.5 | ZIP |

</div>

<sub>Version information is based on the project handover notes. Verify each binary and build before publishing a release.</sub>

---

## Installation

Download the latest build from the [GitHub Releases](https://github.com/ayush13-0/N5-Kanji-Dojo/releases) page.

| Platform | Expected release asset | Guide |
|:---|:---|:---:|
| Android | `N5-Kanji-Dojo-v1.6.apk` | [Install](docs/INSTALL_ANDROID.md) |
| Windows | `N5-Kanji-Dojo-Setup-v1.6.exe` | [Install](docs/INSTALL_WINDOWS.md) |
| macOS (Apple Silicon) | `N5-Kanji-Dojo-v1.5-mac-AppleSilicon.zip` | [Install](docs/INSTALL_MACOS.md) |
| macOS (Intel) | `N5-Kanji-Dojo-v1.5-mac-Intel.zip` | [Install](docs/INSTALL_MACOS.md) |

> **Note:** Filenames above are expected names, not verified download links. Binary packages must be uploaded to GitHub Releases before downloads are available.

### Japanese Pronunciation

Pronunciation uses text-to-speech provided by the host operating system. Voice availability depends on the installed Japanese language and speech packages. See the [Audio Setup guide](docs/AUDIO_SETUP.md) for configuration and troubleshooting.

---

## Repository Structure

The repository is intended to contain the application source and platform-specific project files.

| Directory | Purpose |
|:---|:---|
| `app/` | Core application |
| `android/` | Android application source |
| `windows/` | Windows application source |
| `mac/` | macOS application source |
| `assets/` | Images and application resources |
| `docs/` | Technical and installation documentation |
| `scripts/` | Build and release automation |

<sub>Confirm the source tree and technology stack against the checked-in files. This table describes intended organization, not a verified dependency inventory.</sub>

---

## Documentation

<div align="center">

| Document | Description |
|:---|:---|
| [VERSIONS.md](VERSIONS.md) | Platform version information |
| [CHANGELOG.md](CHANGELOG.md) | Release history |
| [ARCHITECTURE.md](docs/ARCHITECTURE.md) | Application architecture |
| [BUILD_ANDROID.md](docs/BUILD_ANDROID.md) | Android build instructions |
| [BUILD_WINDOWS.md](docs/BUILD_WINDOWS.md) | Windows build instructions |
| [BUILD_MACOS.md](docs/BUILD_MACOS.md) | macOS build instructions |
| [RELEASE_PROCESS.md](docs/RELEASE_PROCESS.md) | Release preparation |
| [AUDIO_SETUP.md](docs/AUDIO_SETUP.md) | Japanese speech configuration |
| [FAQ.md](docs/FAQ.md) | Frequently asked questions |
| [SECURITY.md](SECURITY.md) | Security policy |
| [PRIVACY.md](PRIVACY.md) | Privacy statement |
| [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md) | Third-party attribution |

</div>

---

## Demo

A demonstration video can be placed at `demo/Demo-Kanji-Dojo.mp4`, with a GIF preview at `assets/demo/kanji-dojo-demo.gif`. See [demo/README.md](demo/README.md) for instructions.

---

## Contributing

Feedback is welcome.

- **Report a bug:** open an [issue](https://github.com/ayush13-0/N5-Kanji-Dojo/issues).
- **Suggest an improvement:** open an issue describing the use case.
- **Support the project:** star the repository and share it with Japanese learners.

---

## Security and Privacy

- Report vulnerabilities as described in [SECURITY.md](SECURITY.md).
- Data handling is described in [PRIVACY.md](PRIVACY.md).
- Do not commit signing keys, passwords, API tokens, or private credentials.
- Verify third-party dependencies and distribution permissions before releasing binaries.

---

## License

See [LICENSE](LICENSE). The current repository license reserves all rights and does not grant general redistribution or modification permissions. Confirm ownership and third-party licensing obligations before changing it.

---

<div align="center">

**Developed by Ayush Kumar**

<a href="https://github.com/ayush13-0"><img src="https://img.shields.io/badge/GitHub-ayush13--0-243B55?style=flat-square&logo=github&logoColor=white" alt="GitHub"></a>
<a href="https://www.linkedin.com/"><img src="https://img.shields.io/badge/LinkedIn-Connect-243B55?style=flat-square&logo=linkedin&logoColor=white" alt="LinkedIn"></a>

<sub>© Ayush Kumar · N5 Kanji Dojo</sub>

<img src="https://capsule-render.vercel.app/api?type=rect&color=0:141E30,100:243B55&height=50&section=footer" width="100%" alt="">

</div>
