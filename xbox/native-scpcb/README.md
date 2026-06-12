# SCPCB360 Native XEX Facility Build

This is the first native Xbox 360/XDK build path for the SCP:CB port.

It is not a direct compile of the C# MonoGame project. It is a native tech demo that ports the current desktop tech-demo behavior into an XDK executable.

## Included

- Native Xbox 360 `.xex` output.
- Native main menu with controller start flow.
- Native loading screens with progress updates while the facility is generated and RMESH files load.
- Native intro sequence pass:
  - A/Start begins the 173 intro from the main menu.
  - Y still jumps directly to the generated facility tech demo.
  - Loads the real `173bright` RMESH intro room.
  - Spawns the player in the Class-D cell area.
  - Runs wake-up, escort, chamber entry, test, blackout, 173 breach, and facility handoff beats.
  - Uses the real `.x` CB door model set for visible intro doors when the XDK mesh loader accepts the files.
  - Loads intro-only NPC assets during the intro loading step, then uses a native static `.b3d` reader for visible intro NPC models:
    - `GFX\npcs\173_2.b3d`
    - `GFX\npcs\guard.b3d`
    - `GFX\npcs\classd.b3d`
    - `GFX\npcs\clerk.b3d`
  - Falls back to simple native geometry if an NPC model fails to load.
  - Queues intro NPCs as lightweight model instances instead of rebuilding their full triangle surfaces into scripted geometry every frame.
  - Shows scripted intro subtitles and breach flicker/blackout effects.
  - Hands off into the generated facility after the breach beat.
- Direct3D 720p rendering.
- `MapSystem.bb`-style procedural layout generation:
  - seeded grid carving
  - LCZ, HCZ, EZ zone bands
  - room shape counting
  - key room slot assignment
  - checkpoints
  - 173/start placement
- Runtime attempt to load real room RMESH files from `GFX\map`.
- Runtime attempt to load RMESH surface textures from `GFX\map`.
- Runtime preload of the real CB `.x` door/button model set through XDK D3DX:
  - `Door01.x`
  - `DoorFrame.x`
  - `Button.x`
  - `ButtonKeycard.x`
  - `ButtonCode.x`
  - `ButtonScanner.x`
  - `heavydoor1.x`
  - `heavydoor2.x`
  - `ContDoorLeft.x`
  - `ContDoorRight.x`
- Runtime deferred load of selected real CB `.b3d` NPC models through a native Blitz3D chunk reader.
  - Static bind-pose geometry is parsed from `TEXS`, `BRUS`, `NODE`, `MESH`, `VRTS`, and `TRIS`.
  - `BONE`, `ANIM`, and `KEYS` chunks are skipped for now.
- Fallback room geometry if facility RMESH loading fails.
- Controller movement with left stick.
- Controller looking with right stick.
- CB-style player vitals:
  - left trigger sprint drains stamina and exhaustion temporarily prevents sprinting
  - stamina regenerates more slowly while moving and faster while standing still
  - B toggles crouch, lowers the camera, and slows movement
  - right shoulder manually blinks
  - automatic blink timing uses the original short close/hold/open style timing
  - blink blackout is rendered over the world
  - bottom HUD meters show eye/blink state, stamina, crouch/sprint state, and step cadence
- Basic native player-vs-facility triangle collision with room-level broadphase culling.
- Basic textured RMESH rendering with a white fallback texture for missing/unsupported images.
- Render-distance culling around the player so the full facility is not drawn every frame.
- Xbox performance pass:
  - default world render distance is reduced to a CB-fog-style range for fewer room submissions
  - room surface culling now uses circular distance around the player instead of a wide square
  - intro NPC model instances are distance/front culled before drawing
  - stamina/blink HUD meter rectangles are folded into the normal HUD batch instead of separate draw calls
  - FPS HUD now uses raw wall-clock frame time instead of clamped gameplay simulation time, and also shows milliseconds per frame
- Safe lighting recovery pass:
  - `.x` model vertices are CPU-lit before submission, using a simple top/front light direction.
  - a small full-screen ambient darkening pass restores some CB-style darkness without reintroducing the shader fog/lighting path that crashed hardware.
- CB-style generated facility doors:
  - doors are placed from the original `MapSystem.bb` room-shape/yaw adjacency rules
  - LCZ/EZ doors use normal split-panel behavior
  - HCZ doors use the heavier door motion profile
  - doors track `open`, `openstate`, `locked`, `keycard`, `code`, `timer`, `timerstate`, `fastopen`, and `AutoClose`-style fields
  - open/close animation advances through the original 0-180 state range
  - panels slide apart using the original sine-shaped movement curve
  - door frames, split panels, heavy-door pieces, and button/keypad panels use real `.x` model geometry when available
  - native box geometry remains as a fallback if model loading fails
  - moving/closed panels collide with the player
  - `A` uses the nearest finished door button and rejects locked/keycard/code doors
- Back button returns from gameplay to the native main menu.
- HUD text:
  - `SCPCB360 TECH DEMO XEX`
  - FPS
  - player coordinates
  - facility load status
  - collision hit status
  - texture load count
  - map load failure count
  - render distance, submitted room surface batches, submitted triangle count, and door count
- Generated A-button beep through XAudio2.
- Short rumble pulse when A is pressed.

## Not Yet Included

- A proper baked Xbox texture/content pipeline. This build loads original image files at runtime through XDK D3DX.
- Lightmap/detail multi-texture blending. The native renderer currently chooses one RMESH texture per surface.
- SCP-style shader fog and directional lighting. The first shader implementation crashed on hardware and remains disabled; this build uses CPU-lit models plus a safe screen darkening pass instead.
- Full capsule step/slide collision and stairs.
- Animated B3D NPC playback. The intro now renders selected real NPC model geometry, but skeleton animation and scripted animation frames are not ported yet.
- OGG music, voice, and ambient playback. The original intro audio files exist, but this native XEX currently only has generated PCM cue beeps through XAudio2.
- Real breath, heartbeat, and footstep audio tied to the new stamina/crouch/step state.
- SCP-173 AI driven by blink/line-of-sight state.
- Collision mesh use from `DoorColl.x`; door collision still uses the existing native oriented-panel collision.
- Static `.b3d` intro props beyond selected NPCs. Most props and all skeletal animation still need additional import work.
- Door sounds from `SFX\Door\*.ogg`; the native audio path still needs decoded PCM/XMA assets.
- Inventory-backed keycards, code-entry UI, hand scanners, linked doors, elevators, room-scripted locked states, event placement, decals, scripted room props, NPC AI, or gameplay systems from the later parts of `MapSystem.bb`.
- Any fake conversion from `.NET 9` or MonoGame DesktopGL.

## Build

From the repository root:

```bat
xbox\native-scpcb\build-native-scpcb.cmd
```

Expected output:

- `xbox\native-scpcb\scpcb360_native.xex`
- `xbox\native-scpcb\scpcb360_native.exe`
- `xbox\native-scpcb\scpcb360_native.pe`
- `xbox\native-scpcb\scpcb360_native.xdb`
- copied runtime room assets: `xbox\native-scpcb\GFX\map\*.rmesh`
- copied runtime model assets: `xbox\native-scpcb\GFX\map\*.x`
- copied selected runtime NPC assets:
  - `xbox\native-scpcb\GFX\npcs\173_2.b3d`
  - `xbox\native-scpcb\GFX\npcs\guard.b3d`
  - `xbox\native-scpcb\GFX\npcs\classd.b3d`
  - `xbox\native-scpcb\GFX\npcs\clerk.b3d`
- copied selected runtime NPC textures referenced by those models, plus a small safety set for alternate Class-D/clerk skins.
- copied runtime textures: `xbox\native-scpcb\GFX\map\*.jpg`, `*.jpeg`, `*.png`, `*.bmp`, `*.dds`, `*.tga`
- copied menu images: `xbox\native-scpcb\GFX\menu\*.jpg`, `*.png`
- copied loading images: `xbox\native-scpcb\Loadingscreens\*.jpg`, `*.png`

## Deploy

Copy these to the Xbox 360 together:

- `xbox\native-scpcb\scpcb360_native.xex`
- the entire `xbox\native-scpcb\GFX` folder
- the entire `xbox\native-scpcb\Loadingscreens` folder

If the HUD shows non-zero texture failures, at least one original image format or filename did not load through D3DX on the console and should be converted to an Xbox-friendly DDS in a later pass.

The intro voice and music assets under `SFX\Room\Intro` and `SFX\Music\Intro.ogg` are not copied for runtime yet because the native build does not decode OGG. The next audio pass should convert required intro clips to a console-friendly PCM/WAV or XMA path and load them through XAudio2.
