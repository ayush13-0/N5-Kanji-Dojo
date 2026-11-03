#!/usr/bin/env python3
"""Render the Dojo icon (white あ on indigo, vermillion dot — same art as
Android/Windows) at all macOS sizes and pack a PNG-based .icns by hand."""
import io, os, struct
from PIL import Image, ImageDraw, ImageFont

FONT = os.path.expanduser("~/workspace/apk-build/fonts/NotoSansCJKjp-Bold.otf")
OUT = os.path.expanduser("~/workspace/mac-build/icon.icns")

def render(size):
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
    if size != S:
        img = img.resize((size, size), Image.LANCZOS)
    buf = io.BytesIO()
    img.save(buf, format="PNG")
    return buf.getvalue()

# icns OSType -> pixel size (PNG entries)
TYPES = [(b"icp4", 16), (b"icp5", 32), (b"icp6", 64), (b"ic07", 128),
         (b"ic08", 256), (b"ic09", 512), (b"ic10", 1024),
         (b"ic11", 32), (b"ic12", 64), (b"ic13", 256), (b"ic14", 512)]
entries = b""
for ostype, size in TYPES:
    png = render(size)
    entries += ostype + struct.pack(">I", len(png) + 8) + png
icns = b"icns" + struct.pack(">I", len(entries) + 8) + entries
with open(OUT, "wb") as f:
    f.write(icns)
print("wrote", OUT, len(icns), "bytes")
