# Privacy

## Overview

N5 Kanji Dojo is described in its project handover as an offline-first Japanese study application. The handover states that learning progress, settings, quiz history and user-added vocabulary are stored locally on the device.

## Data described as stored locally

- Learned-kanji progress.
- Application settings.
- Quiz history.
- User-created vocabulary.

The handover also describes an erase option in Settings.

## Network and external links

The Android v1.6 handover states that the APK requests zero permissions and does not include the Android `INTERNET` permission. It also states that LinkedIn and GitHub links open in the external browser.

These statements must be rechecked against the exact release build. This document is not a substitute for a source-code review, traffic analysis or independent privacy audit.

## Speech services

Speech output uses platform speech facilities according to the supplied handover:

- Android: the device's configured Japanese text-to-speech voice.
- Windows: installed Windows speech voices.
- macOS: the macOS `say` speech engine.

Operating-system speech components may have their own settings and privacy behaviour. Review the platform provider's documentation.

## User responsibility

Do not add analytics, advertising, cloud synchronisation, telemetry or remote services without updating the application, this policy and the release documentation. Explain clearly what information is transmitted and obtain any consent required by applicable law.

## Contact

For privacy questions, contact the maintainer through the public GitHub profile: https://github.com/ayush13-0
