#!/usr/bin/env python3
"""
cook_assets.py — PC-side asset cooking pipeline for SCP:CB Xbox 360 port.

Workflow:
  1. Reads the extracted .obj room meshes (output from rmesh_extractor.py).
  2. Converts .obj → .fbx via Blender's headless CLI (FBX is the XNA content pipeline's
     preferred input; .obj lacks material embedding and doesn't survive the content
     processor cleanly).
  3. Invokes the XNA Content Pipeline's xnabuild.exe (MonoGame's MGCB equivalent)
     to bake .fbx → .xnb with PowerPC vertex alignment.
  4. Converts .jpg/.png textures → DXT1 (opaque) or DXT5 (alpha) .xnb via MGCB.
  5. Converts .ogg/.wav audio → XMA (hardware-accelerated Xbox 360 audio) .xnb.

Requirements (PC side, run before deploying to 360):
  - Python 3.11+
  - Blender 4.x installed and `blender` on PATH
  - MonoGame Content Builder (mgcb) installed: https://docs.monogame.net/articles/tools/mgcb.html
  - xmaencode.exe from the Xbox 360 XDK (or the open-source xmaencoder wrapper)

Usage:
  python cook_assets.py --src "C:/scpcb_extracted" --out "C:/scpcb360_content"
"""

import argparse
import json
import os
import shutil
import subprocess
import sys
from pathlib import Path
from concurrent.futures import ThreadPoolExecutor, as_completed

# ── Configuration ──────────────────────────────────────────────────────────────

# MonoGame Content Builder executable — adjust path as needed
MGCB_EXE = shutil.which("mgcb") or r"C:\Program Files\MonoGame\v3.0\Tools\mgcb.exe"

# Blender headless executable
BLENDER_EXE = shutil.which("blender") or r"C:\Program Files\Blender Foundation\Blender 4.1\blender.exe"

# XMA encoder (from XDK or community wrapper)
XMA_EXE = shutil.which("xmaencode") or r"C:\XDK\bin\xmaencode.exe"

# Content output subdirectories
MESH_SUBDIR    = "GFX/map"
TEXTURE_SUBDIR = "GFX"
AUDIO_SUBDIR   = "SFX"

# DXT format selection threshold: if alpha channel variance > this, use DXT5
DXT5_THRESHOLD = 5

# Parallel worker count (CPU-bound; use logical core count)
WORKERS = os.cpu_count() or 4

# ── Blender OBJ→FBX conversion script (injected as inline Python) ─────────────

BLENDER_CONVERT_PY = """
import bpy, sys, os

argv = sys.argv
argv = argv[argv.index("--") + 1:]
src_obj = argv[0]
dst_fbx = argv[1]

bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.obj(filepath=src_obj, split_mode='OFF')

# Flip Z-up (OBJ) → Y-up (XNA/FBX)
for obj in bpy.context.scene.objects:
    obj.rotation_euler = (0, 0, 0)

bpy.ops.export_scene.fbx(
    filepath=dst_fbx,
    use_selection=False,
    axis_forward='-Z',
    axis_up='Y',
    apply_unit_scale=True,
    global_scale=0.01,          # Blitz3D units → metres
    mesh_smooth_type='FACE',
)
print(f"[cook] Converted {src_obj} -> {dst_fbx}")
"""


# ─── Main entry ────────────────────────────────────────────────────────────────

def parse_args():
    p = argparse.ArgumentParser(description="SCP:CB Xbox 360 asset cooker")
    p.add_argument("--src",  required=True, help="Root of extracted CB assets")
    p.add_argument("--out",  required=True, help="Output XNB content directory")
    p.add_argument("--jobs", type=int, default=WORKERS, help="Parallel workers")
    p.add_argument("--dry",  action="store_true", help="Print commands, don't run them")
    p.add_argument("--skip-mesh",  action="store_true")
    p.add_argument("--skip-tex",   action="store_true")
    p.add_argument("--skip-audio", action="store_true")
    return p.parse_args()


def main():
    args  = parse_args()
    src   = Path(args.src)
    out   = Path(args.out)
    out.mkdir(parents=True, exist_ok=True)

    results = {"ok": 0, "fail": 0, "skip": 0}

    print(f"[cook] Source : {src}")
    print(f"[cook] Output : {out}")
    print(f"[cook] Workers: {args.jobs}")
    print()

    # 1. Mesh pipeline: .obj → .fbx → .xnb
    if not args.skip_mesh:
        objs = list((src / "GFX" / "map").glob("*.obj"))
        print(f"[cook] Meshes: {len(objs)} .obj files")
        with ThreadPoolExecutor(max_workers=args.jobs) as pool:
            futs = {pool.submit(cook_mesh, f, out, args.dry): f for f in objs}
            for fut in as_completed(futs):
                ok, msg = fut.result()
                if ok:  results["ok"] += 1
                else:   results["fail"] += 1; print(f"  FAIL {msg}")

    # 2. Texture pipeline: .jpg/.png → .xnb (DXT1/DXT5)
    if not args.skip_tex:
        textures = list((src / "GFX").rglob("*.jpg")) + list((src / "GFX").rglob("*.png"))
        # Skip textures that belong to the map folder (handled separately by mesh)
        textures = [t for t in textures if "map" not in str(t)]
        print(f"\n[cook] Textures: {len(textures)} image files")
        with ThreadPoolExecutor(max_workers=args.jobs) as pool:
            futs = {pool.submit(cook_texture, f, src, out, args.dry): f for f in textures}
            for fut in as_completed(futs):
                ok, msg = fut.result()
                if ok:  results["ok"] += 1
                else:   results["fail"] += 1; print(f"  FAIL {msg}")

    # 3. Audio pipeline: .ogg/.wav → XMA .xnb
    if not args.skip_audio:
        audio = list((src / "SFX").rglob("*.ogg")) + list((src / "SFX").rglob("*.wav"))
        print(f"\n[cook] Audio: {len(audio)} audio files")
        with ThreadPoolExecutor(max_workers=args.jobs) as pool:
            futs = {pool.submit(cook_audio, f, src, out, args.dry): f for f in audio}
            for fut in as_completed(futs):
                ok, msg = fut.result()
                if ok:  results["ok"] += 1
                else:   results["fail"] += 1; print(f"  FAIL {msg}")

    print(f"\n[cook] Done — OK={results['ok']}  FAIL={results['fail']}")
    sys.exit(0 if results["fail"] == 0 else 1)


# ─── Mesh cooker ───────────────────────────────────────────────────────────────

def cook_mesh(obj_path: Path, out_root: Path, dry: bool) -> tuple[bool, str]:
    stem    = obj_path.stem
    fbx_tmp = obj_path.with_suffix(".fbx")
    xnb_dir = out_root / MESH_SUBDIR
    xnb_dir.mkdir(parents=True, exist_ok=True)
    xnb_out = xnb_dir / stem  # MGCB adds .xnb extension itself

    # Step A: OBJ → FBX via Blender headless
    blender_cmd = [
        BLENDER_EXE, "--background", "--factory-startup",
        "--python-expr", BLENDER_CONVERT_PY,
        "--", str(obj_path), str(fbx_tmp),
    ]
    ok, err = run(blender_cmd, dry)
    if not ok:
        return False, f"{stem}: Blender failed — {err}"

    # Step B: FBX → XNB via MGCB
    # Target platform Xbox360 uses PowerPC alignment; MGCB handles byte order.
    mgcb_cmd = [
        MGCB_EXE,
        f"/platform:Xbox360",
        f"/outputDir:{xnb_dir}",
        f"/build:{fbx_tmp};ModelProcessor",
    ]
    ok, err = run(mgcb_cmd, dry)
    if not dry:
        fbx_tmp.unlink(missing_ok=True)  # clean temp .fbx

    if not ok:
        return False, f"{stem}: MGCB mesh failed — {err}"
    return True, stem


# ─── Texture cooker ────────────────────────────────────────────────────────────

def cook_texture(tex_path: Path, src_root: Path, out_root: Path, dry: bool) -> tuple[bool, str]:
    rel     = tex_path.relative_to(src_root)
    xnb_dir = (out_root / rel).parent
    xnb_dir.mkdir(parents=True, exist_ok=True)

    # Determine DXT format by checking alpha channel
    processor_param = detect_dxt_format(tex_path)

    mgcb_cmd = [
        MGCB_EXE,
        f"/platform:Xbox360",
        f"/outputDir:{xnb_dir}",
        f"/processorParam:ColorKeyEnabled=False",
        f"/processorParam:GenerateMipmaps=True",
        f"/processorParam:TextureFormat={processor_param}",
        f"/build:{tex_path};TextureProcessor",
    ]
    ok, err = run(mgcb_cmd, dry)
    if not ok:
        return False, f"{tex_path.name}: texture cook failed — {err}"
    return True, tex_path.name


def detect_dxt_format(path: Path) -> str:
    """Use pillow to check if image has meaningful alpha → DXT5, else DXT1."""
    try:
        from PIL import Image
        import numpy as np
        img = Image.open(path).convert("RGBA")
        alpha = np.array(img)[:, :, 3]
        if alpha.min() < 250 and alpha.std() > DXT5_THRESHOLD:
            return "Dxt5"
        return "Dxt1"
    except ImportError:
        # No pillow available — default to DXT5 (safe for all textures, slightly larger)
        return "Dxt5"
    except Exception:
        return "Dxt5"


# ─── Audio cooker ──────────────────────────────────────────────────────────────

def cook_audio(audio_path: Path, src_root: Path, out_root: Path, dry: bool) -> tuple[bool, str]:
    rel     = audio_path.relative_to(src_root)
    xnb_dir = (out_root / rel).parent
    xnb_dir.mkdir(parents=True, exist_ok=True)

    # Convert .ogg → .wav first (XMA encoder needs PCM WAV input)
    wav_tmp = audio_path.with_suffix(".tmp.wav")
    if audio_path.suffix.lower() == ".ogg":
        ffmpeg_cmd = [
            "ffmpeg", "-y", "-i", str(audio_path),
            "-ar", "44100", "-ac", "2",
            str(wav_tmp),
        ]
        ok, err = run(ffmpeg_cmd, dry)
        if not ok:
            return False, f"{audio_path.name}: ffmpeg failed — {err}"
        wav_in = wav_tmp
    else:
        wav_in = audio_path

    # Encode WAV → XMA2 via xmaencode
    xma_out = (xnb_dir / audio_path.stem).with_suffix(".xma")
    xma_cmd = [
        XMA_EXE,
        str(wav_in),
        f"/X:{xma_out}",
        "/StereoMode:2",        # XMA2 stereo
        "/Quality:75",
    ]
    ok, err = run(xma_cmd, dry)
    if not dry:
        wav_tmp.unlink(missing_ok=True)

    if not ok:
        # Fallback: let MGCB use its built-in WMA processor instead
        mgcb_cmd = [
            MGCB_EXE,
            f"/platform:Xbox360",
            f"/outputDir:{xnb_dir}",
            f"/build:{wav_in};SoundEffectProcessor",
        ]
        ok, err = run(mgcb_cmd, dry)
        if not ok:
            return False, f"{audio_path.name}: audio cook failed — {err}"

    return True, audio_path.name


# ─── Shell runner ──────────────────────────────────────────────────────────────

def run(cmd: list, dry: bool) -> tuple[bool, str]:
    """Run a shell command. Returns (success, stderr_or_empty)."""
    if dry:
        print("  DRY:", " ".join(str(c) for c in cmd))
        return True, ""
    try:
        result = subprocess.run(
            cmd,
            capture_output=True,
            text=True,
            timeout=120,
        )
        if result.returncode != 0:
            return False, result.stderr.strip()[:200]
        return True, ""
    except FileNotFoundError as e:
        return False, f"Executable not found: {e}"
    except subprocess.TimeoutExpired:
        return False, "Timeout after 120s"


if __name__ == "__main__":
    main()
