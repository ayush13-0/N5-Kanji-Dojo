# Architecture Overview

This document captures the high-level product behaviour described in the project handover. It is not a substitute for source-code documentation.

## Product areas

- Learning content: kana, kanji and vocabulary.
- Practice: 15 quiz modes, including Vocabulary Course and Fill the Missing Kanji.
- Personal vocabulary: user-created words persisted locally.
- Progress: learned kanji, settings and quiz history.
- Search and reference: vocabulary search and kanji pages.
- Export: CSV export for vocabulary / study data.
- Audio: operating-system speech integration.
- Presentation: compact interface and Mobile / Desktop layout preference.

## Persistence

The handover describes local persistence for user progress, settings, quiz history and custom vocabulary. It also describes an erase option in Settings.

Document the actual storage engine, schema, migration strategy, backup behaviour and data deletion implementation here once the source code is added.

## Platform integration

The handover describes native speech integration for Android, Windows and macOS. Record the exact bridge implementation, IPC boundaries, permissions and failure handling from the actual source before publishing a technical security claim.

## Source layout

The `app/` and `src/` folders are reserved for the real application and platform source. They are not populated with fabricated implementation code in this documentation-only package.
