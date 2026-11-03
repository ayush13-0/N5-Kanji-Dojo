#!/bin/bash
# Manual APK build v1.1 for N5 Kanji Dojo — dependency-free shell.
# Same SDK build-tools pipeline as v1.0 (aapt2/javac/d8/zipalign/apksigner),
# but no androidx.webkit: javac/d8 handle only the app's own classes.
set -e
cd "$(dirname "$0")"
export JAVA_HOME=$HOME/workspace/apk-build/jdk17
export PATH=$JAVA_HOME/bin:$PATH
AH=$HOME/workspace/apk-build/android-sdk
BT=$AH/build-tools/36.0.0
AJAR=$AH/platforms/android-36/android.jar
APP=dojo-app
OUT=$APP/build-manual

echo "== aapt2 compile + link =="
rm -rf "$OUT"
mkdir -p "$OUT/compiled" "$OUT/classes" "$OUT/dex" "$OUT/final"
$BT/aapt2 compile --dir "$APP/app/src/main/res" -o "$OUT/compiled/res.zip"
$BT/aapt2 link -o "$OUT/unsigned.apk" \
  -I "$AJAR" \
  --manifest "$APP/app/src/main/AndroidManifest.xml" \
  --min-sdk-version 26 --target-sdk-version 36 \
  --version-code 6 --version-name 1.6 \
  -A "$APP/app/src/main/assets" \
  "$OUT/compiled/res.zip"
echo "linked"

echo "== javac (android.jar only) =="
find "$APP/app/src/main/java" -name "*.java" > "$OUT/sources.txt"
javac -source 17 -target 17 -encoding UTF-8 \
  -cp "$AJAR" \
  -d "$OUT/classes" @"$OUT/sources.txt"
echo "compiled"

echo "== d8 dex (app classes only) =="
$BT/d8 --release --lib "$AJAR" --output "$OUT/dex" \
  $(find "$OUT/classes" -name "*.class")
ls -lh "$OUT/dex"

echo "== add dex, align, sign =="
cp "$OUT/unsigned.apk" "$OUT/withdex.apk"
cd "$OUT/dex" && zip -q -X "../withdex.apk" classes.dex && cd - > /dev/null
$BT/zipalign -f -p 4 "$OUT/withdex.apk" "$OUT/aligned.apk"
PASSFILE=$HOME/workspace/apk-build/signing/pass.txt
$BT/apksigner sign --v1-signing-enabled true --v2-signing-enabled true --v3-signing-enabled true \
  --ks "$HOME/workspace/apk-build/signing/n5-kanji-dojo-release.jks" \
  --ks-key-alias n5dojo \
  --ks-pass "file:$PASSFILE" --key-pass "file:$PASSFILE" \
  --out "$OUT/final/N5-Kanji-Dojo-v1.6.apk" \
  "$OUT/aligned.apk"
echo "== verify =="
$BT/apksigner verify --verbose "$OUT/final/N5-Kanji-Dojo-v1.6.apk" | head -6
$BT/aapt2 dump badging "$OUT/final/N5-Kanji-Dojo-v1.6.apk" | head -6
echo "PERMISSIONS:"
$BT/aapt2 dump permissions "$OUT/final/N5-Kanji-Dojo-v1.6.apk" || true
ls -lh "$OUT/final/N5-Kanji-Dojo-v1.6.apk"
echo "MANUAL_BUILD_V11_DONE"
