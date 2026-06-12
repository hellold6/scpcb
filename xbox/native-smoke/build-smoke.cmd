@echo off
setlocal
set "XEDK=C:\Program Files (x86)\Microsoft Xbox 360 SDK"
call "%XEDK%\bin\win32\xdkvars.bat"
cd /d "C:\Users\Willi\scpcb\xbox\native-smoke"
cl /nologo /EHsc /D_XBOX /I"%XEDK%\include\xbox" smoke.cpp /link /nologo /LIBPATH:"%XEDK%\lib\xbox" /OUT:smoke.exe
if errorlevel 1 exit /b %errorlevel%
imagexex /IN:smoke.exe /OUT:smoke.xex
exit /b %errorlevel%
