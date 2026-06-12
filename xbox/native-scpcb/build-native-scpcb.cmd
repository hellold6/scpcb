@echo off
setlocal
set "XEDK=C:\Program Files (x86)\Microsoft Xbox 360 SDK"
call "%XEDK%\bin\win32\xdkvars.bat"
cd /d "C:\Users\Willi\scpcb\xbox\native-scpcb"

if not exist "GFX\map" mkdir "GFX\map"
if not exist "GFX\menu" mkdir "GFX\menu"
if not exist "GFX\npcs" mkdir "GFX\npcs"
if not exist "Loadingscreens" mkdir "Loadingscreens"
if exist "..\..\GFX\map\*.rmesh" copy /Y "..\..\GFX\map\*.rmesh" "GFX\map\" >nul
if exist "..\..\GFX\map\*.x" copy /Y "..\..\GFX\map\*.x" "GFX\map\" >nul
for %%e in (jpg jpeg png bmp dds tga) do if exist "..\..\GFX\map\*.%%e" copy /Y "..\..\GFX\map\*.%%e" "GFX\map\" >nul
del /Q "GFX\npcs\*.b3d" >nul 2>nul
for %%e in (jpg jpeg png bmp dds tga pt) do del /Q "GFX\npcs\*.%%e" >nul 2>nul
for %%f in (173_2.b3d guard.b3d classd.b3d clerk.b3d) do if exist "..\..\GFX\npcs\%%f" copy /Y "..\..\GFX\npcs\%%f" "GFX\npcs\" >nul
for %%f in (173texture.jpg guard_diffuse.jpg helmet_guard.jpg MTF_P90_diffuse02.jpg papertexture.jpg classd1.jpg classd2.jpg classd3.jpg clerk_d1.jpg scientist.jpg scientist2.jpg gonzales.jpg janitor.jpg 0.pt) do if exist "..\..\GFX\npcs\%%f" copy /Y "..\..\GFX\npcs\%%f" "GFX\npcs\" >nul
for %%e in (jpg jpeg png bmp dds tga) do if exist "..\..\GFX\menu\*.%%e" copy /Y "..\..\GFX\menu\*.%%e" "GFX\menu\" >nul
for %%e in (jpg jpeg png bmp dds tga) do if exist "..\..\Loadingscreens\*.%%e" copy /Y "..\..\Loadingscreens\*.%%e" "Loadingscreens\" >nul

cl /nologo /EHsc /D_XBOX /I"%XEDK%\include\xbox" scpcb360_native.cpp ^
    /link /nologo /LIBPATH:"%XEDK%\lib\xbox" ^
    d3d9.lib d3dx9.lib xgraphics.lib xapilib.lib xinput2.lib xaudio2.lib xmcore.lib xboxkrnl.lib ^
    /OUT:scpcb360_native.exe
if errorlevel 1 exit /b %errorlevel%

imagexex /IN:scpcb360_native.exe /OUT:scpcb360_native.xex
exit /b %errorlevel%
