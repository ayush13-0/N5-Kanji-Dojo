#!/usr/bin/env python3
"""Render the N5 Kanji Dojo Windows icon: white hiragana あ on indigo,
vermillion dot — same artwork as the Android launcher icon, as .ico."""
from PIL import Image, ImageDraw, ImageFont
import os

FONT = os.path.expanduser("~/workspace/apk-build/fonts/NotoSansCJKjp-Bold.otf")
OUT = os.path.expanduser("~/workspace/win-build/dojo-win/app.ico")
S = 1024
img = Image.new("RGBA", (S, S), (0, 0, 0, 0))
d = ImageDraw.Draw(img)
d.rounded_rectangle([0, 0, S - 1, S - 1], radius=int(S * 0.18), fill=(38, 51, 102, 255))
font = ImageFont.truetype(FONT, int(S * 0.72))
bbox = d.textbbox((0, 0), "あ", font=font)
w, h = bbox[2] - bbox[0], bbox[3] - bbox[1]
d.text(((S - w) / 2 - bbox[0], (S - h) / 2 - bbox[1] - S * 0.03), "あ",
       font=font, fill=(255, 247, 230, 255))
r = int(S * 0.055)
cx, cy = int(S * 0.76), int(S * 0.24)
d.ellipse([cx - r, cy - r, cx + r, cy + r], fill=(198, 62, 46, 255))
img.save(OUT, format="ICO",
         sizes=[(16, 16), (24, 24), (32, 32), (48, 48), (64, 64), (128, 128), (256, 256)])
print("wrote", OUT, os.path.getsize(OUT), "bytes")
