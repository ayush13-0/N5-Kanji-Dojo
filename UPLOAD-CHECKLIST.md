# Final GitHub Upload Checklist

## A. Before upload

- [ ] Add the actual application source to `app/` and `src/`.
- [ ] Add actual platform build files and dependency lockfiles.
- [ ] Add exact third-party licence texts to `licenses/`.
- [ ] Confirm the final LinkedIn profile URL in `README.md`.
- [ ] Add `Demo-Kanji-Dojo.mp4` to `demo/` if permitted.
- [ ] Review `LICENSE` and choose the intended distribution terms.
- [ ] Remove any placeholders that no longer apply.

## B. Create the GitHub repository

Create an empty repository named `N5-Kanji-Dojo` under `ayush13-0`.
Do not initialise it with a README, licence or .gitignore because this package already contains them.

## C. Upload from Windows PowerShell

Extract this ZIP, open PowerShell in the extracted `N5-Kanji-Dojo` folder, then run:

```powershell
git init
git add .
git status
git commit -m "Initial N5 Kanji Dojo repository documentation"
git branch -M main
git remote add origin https://github.com/ayush13-0/N5-Kanji-Dojo.git
git push -u origin main
```

Review `git status` before committing. Ensure no signing keys, passwords, private files or unlicensed assets are included.

## D. Publish binaries as GitHub Release assets

Upload the actual APK, Windows installer and both Mac ZIPs to a GitHub Release. Do not commit large installers to the source repository.

Generate fresh full SHA-256 hashes and add them to the release description and `checksums-sha256.txt`.

## E. Final verification

- [ ] Repository is public/private as intended.
- [ ] README renders correctly.
- [ ] Source folders contain actual source, not placeholders.
- [ ] Release assets download successfully.
- [ ] Checksums match the downloaded files.
- [ ] Signatures are verified.
- [ ] Demo video plays and has permission for public distribution.
- [ ] Installation guides match the actual released builds.
