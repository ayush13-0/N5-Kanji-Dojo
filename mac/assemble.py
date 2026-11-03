#!/usr/bin/env python3
"""Assemble 'N5 Kanji Dojo.app' for one arch from the stock Electron.app."""
import os, plistlib, shutil, sys

ARCH = sys.argv[1]  # arm64 | x64
BASE = os.path.expanduser("~/workspace/mac-build")
SRC = os.path.join(BASE, f"stage-{ARCH}", "Electron.app")
DSTROOT = os.path.join(BASE, f"build-{ARCH}")
APP = os.path.join(DSTROOT, "N5 Kanji Dojo.app")

if os.path.exists(DSTROOT):
    shutil.rmtree(DSTROOT)
os.makedirs(DSTROOT)
shutil.copytree(SRC, APP, symlinks=True)

contents = os.path.join(APP, "Contents")

# Rename the executable to the app name
macos_dir = os.path.join(contents, "MacOS")
os.rename(os.path.join(macos_dir, "Electron"),
          os.path.join(macos_dir, "N5 Kanji Dojo"))

# Install our app code
resources = os.path.join(contents, "Resources")
default_asar = os.path.join(resources, "default_app.asar")
if os.path.exists(default_asar):
    os.remove(default_asar)
shutil.copytree(os.path.join(BASE, "app"), os.path.join(resources, "app"))
shutil.copy(os.path.join(BASE, "stage-" + ARCH, "LICENSE"),
            os.path.join(resources, "app", "ELECTRON-LICENSE.txt"))
shutil.copy(os.path.join(BASE, "icon.icns"),
            os.path.join(resources, "icon.icns"))

# Bundle metadata
plist_path = os.path.join(contents, "Info.plist")
with open(plist_path, "rb") as f:
    plist = plistlib.load(f)
plist.update({
    "CFBundleDisplayName": "N5 Kanji Dojo",
    "CFBundleName": "N5 Kanji Dojo",
    "CFBundleExecutable": "N5 Kanji Dojo",
    "CFBundleIdentifier": "com.ayush.n5kanjidojo",
    "CFBundleShortVersionString": "1.5.0",
    "CFBundleVersion": "1.5.0",
    "CFBundleIconFile": "icon",
    "LSMinimumSystemVersion": "11.0",
    "NSHighResolutionCapable": True,
    "CFBundleGetInfoString": "N5 Kanji Dojo 1.5.0 — Built by Ayush",
})
with open(plist_path, "wb") as f:
    plistlib.dump(plist, f)

total = sum(os.path.getsize(os.path.join(dp, fn))
            for dp, _, fns in os.walk(APP) for fn in fns)
print(f"{ARCH}: assembled {APP} ({total/1e6:.0f} MB)")
