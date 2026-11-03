#!/bin/bash
# Manual APK build for N5 Kanji Dojo using Android SDK build-tools directly.
# (Gradle's daemon cannot run in this VM, so the classic aapt2/javac/d8
# pipeline is used instead. Every step is the same tooling Gradle calls.)
set -e
cd "$(dirname "$0")"
export JAVA_HOME=$HOME/workspace/apk-build/jdk17
export PATH=$JAVA_HOME/bin:$PATH
AH=$HOME/workspace/apk-build/android-sdk
BT=$AH/build-tools/34.0.0
AJAR=$AH/platforms/android-34/android.jar
APP=dojo-app
OUT=$APP/build-manual
DEPS=$HOME/workspace/apk-build/deps

echo "== fetch androidx.webkit =="
mkdir -p "$DEPS"
if [ ! -f "$DEPS/webkit-classes.jar" ]; then
  wget -q https://dl.google.com/dl/android/maven2/androidx/webkit/webkit/1.11.0/webkit-1.11.0.aar -O "$DEPS/webkit.aar"
  unzip -p "$DEPS/webkit.aar" classes.jar > "$DEPS/webkit-classes.jar"
fi
ls -lh "$DEPS/webkit-classes.jar"

echo "== aapt2 compile + link =="
rm -rf "$OUT"
mkdir -p "$OUT/compiled" "$OUT/classes" "$OUT/dex" "$OUT/final"
$BT/aapt2 compile --dir "$APP/app/src/main/res" -o "$OUT/compiled/res.zip"
$BT/aapt2 link -o "$OUT/unsigned.apk" \
  -I "$AJAR" \
  --manifest "$APP/app/src/main/AndroidManifest.xml" \
  --min-sdk-version 26 --target-sdk-version 34 \
  --version-code 1 --version-name 1.0 \
  -A "$APP/app/src/main/assets" \
  "$OUT/compiled/res.zip"
echo "linked"

echo "== javac =="
find "$APP/app/src/main/java" -name "*.java" > "$OUT/sources.txt"
javac -source 17 -target 17 -encoding UTF-8 \
  -cp "$AJAR:$DEPS/webkit-classes.jar" \
  -d "$OUT/classes" @"$OUT/sources.txt"
echo "compiled"

echo "== d8 dex =="
$BT/d8 --release --lib "$AJAR" --output "$OUT/dex" \
  $(find "$OUT/classes" -name "*.class") "$DEPS/webkit-classes.jar"
ls -lh "$OUT/dex"

echo "== add dex, align, sign =="
cp "$OUT/unsigned.apk" "$OUT/withdex.apk"
cd "$OUT/dex" && zip -q -X "../withdex.apk" classes.dex && cd - > /dev/null
$BT/zipalign -f -p 4 "$OUT/withdex.apk" "$OUT/aligned.apk"
PASSFILE=$HOME/workspace/apk-build/signing/pass.txt
$BT/apksigner sign \
  --ks "$HOME/workspace/apk-build/signing/n5-kanji-dojo-release.jks" \
  --ks-key-alias n5dojo \
  --ks-pass "file:$PASSFILE" --key-pass "file:$PASSFILE" \
  --out "$OUT/final/N5-Kanji-Dojo-v1.0.apk" \
  "$OUT/aligned.apk"
echo "== verify =="
$BT/apksigner verify --verbose "$OUT/final/N5-Kanji-Dojo-v1.0.apk" | head -6
$BT/aapt2 dump badging "$OUT/final/N5-Kanji-Dojo-v1.0.apk" | head -8
echo "PERMISSIONS:"
$BT/aapt2 dump permissions "$OUT/final/N5-Kanji-Dojo-v1.0.apk" || true
ls -lh "$OUT/final/N5-Kanji-Dojo-v1.0.apk"
echo "MANUAL_BUILD_DONE"
