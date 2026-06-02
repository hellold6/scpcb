# Xbox 360 Dependency Audit

This audit is for the current MonoGame port in `C:\Users\Willi\scpcb`.

## Current Desktop Build

- Project: `scpcb.csproj`
- Target framework: `net9.0`
- Public package dependencies:
  - `MonoGame.Framework.DesktopGL` `3.8.*`
  - `MonoGame.Content.Builder.Task` `3.8.*`
- Content platform: `DesktopGL`
- Verified desktop command:
  - `dotnet build --no-restore /p:EnableMGCBItems=false`
- The root desktop project now removes `xbox/**/*.cs` from compilation so copied migration sources do not create duplicate desktop types.
- Current verified desktop result:
  - Build succeeds.
  - One warning remains from `MonoGame.Content.Builder.Task` attempting `dotnet tool restore` while the sandbox cannot read `C:\Users\Willi\AppData\Roaming\NuGet\NuGet.Config`.

## Xbox 360 Blockers

- `net9.0` cannot target Xbox 360 XNA 4.0. Xbox 360 XNA uses the legacy XNA Framework 4.0 profile with Visual Studio 2010-era tooling.
- `MonoGame.Framework.DesktopGL` is not Xbox 360-capable. DesktopGL uses SDL for windowing, OpenGL for graphics, and OpenAL-Soft for audio.
- Public MonoGame console support is not a public Xbox 360 path. Current public MonoGame docs describe DesktopGL/WindowsDX/mobile publicly; console integrations are private/vendor gated and oriented toward modern consoles.
- `Content/Content.mgcb` targets `/platform:DesktopGL`. Xbox 360 content needs Xbox 360/XNA-compatible XNB output, including console-compatible texture/audio formats.
- Runtime texture loading currently uses `Texture2D.FromStream` and raw filesystem paths. Xbox 360 should use prebuilt content loaded through `ContentManager`.
- Runtime RMESH parsing and texture discovery use `System.IO` heavily. This is useful for desktop debugging but should be converted to offline asset baking for Xbox.
- Keyboard/mouse fallback code is desktop-only. Xbox 360 gameplay input should use controller-only code paths.
- `Program.cs` uses console I/O, including `Console.ReadLine()`, which is not appropriate for Xbox execution.
- Original Blitz3D-era native dependencies are Windows DLLs and cannot execute on Xbox 360:
  - `fmod.dll`
  - `FreeImage.dll`
  - `zlibwapi.dll`
  - `BlitzMovie.dll`
  - `cpuid.dll`
  - `dplayx.dll`
- Original declaration files call Windows APIs:
  - `user32.decls`
  - `kernel32.decls`
  - `gdi32.decls`
  - `fmod.decls`
  - `FreeImage.decls`
  - `zlibwapi.decls`
- Audio assets are mostly `.ogg`. Xbox 360 XNA content should be converted to XNB/XMA or XACT-compatible assets.

## Source Incompatibility Hotspots

- `files/RMeshReader.cs`
  - Runtime binary file reads.
  - Runtime recursive texture search.
  - Runtime render mesh creation from RMESH.
- `files/RenderSystem.cs`
  - `Texture2D.FromStream`.
  - Runtime file stream texture loading.
- `files/XInputRouter.cs`
  - Keyboard and mouse APIs.
  - Controller APIs are useful, but the desktop fallback path must be excluded for Xbox.
- `files/SCPCB360Game.cs`
  - Console debug output.
  - `AppDomain.CurrentDomain.BaseDirectory` and `Environment.CurrentDirectory`.
  - Runtime RMESH discovery.
- `files/MapSystem.cs`
  - Runtime RMESH file probing.

## Path Feasibility

- XNA 4.0 Xbox 360 C#:
  - Best conceptual fit for the current `Microsoft.Xna.Framework` API usage.
  - Requires real XNA Game Studio 4.0 Xbox 360 build/deploy tooling.
  - Current local machine does not expose the XNA MSBuild target file required by the attempted build script.
- MonoGame Xbox 360:
  - No public DesktopGL-to-Xbox360 path.
  - Any old/private fork would need to be supplied and verified separately.
- Native XEX homebrew:
  - More realistic for RGH/JTAG homebrew execution as native code, but it is not a direct C# compile.
  - Would require a C++/native engine port or separate managed runtime strategy.
