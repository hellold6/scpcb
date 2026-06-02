param(
    [string]$Configuration = "Debug",
    [string]$Platform = "Xbox 360"
)

$ErrorActionPreference = "Stop"
$xboxRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$logDir = Join-Path $xboxRoot "logs"
$logPath = Join-Path $logDir "xna360-build.txt"
$projectPath = Join-Path $xboxRoot "SCPCB360.Xna360.csproj"

New-Item -ItemType Directory -Force -Path $logDir | Out-Null

function Write-Log($message) {
    $message | Add-Content -Path $logPath -Encoding UTF8
    Write-Host $message
}

Set-Content -Path $logPath -Value "XNA 4.0 Xbox 360 build attempt" -Encoding UTF8
Write-Log "Project: $projectPath"
Write-Log "Configuration: $Configuration"
Write-Log "Platform: $Platform"
Write-Log ""

$xnaTargets = @(
    "${env:ProgramFiles(x86)}\MSBuild\Microsoft\XNA Game Studio\Microsoft.Xna.GameStudio.targets",
    "${env:ProgramFiles}\MSBuild\Microsoft\XNA Game Studio\Microsoft.Xna.GameStudio.targets"
) | Where-Object { $_ -and (Test-Path $_) }

if ($xnaTargets.Count -eq 0) {
    Write-Log "BLOCKER: XNA 4.0 Game Studio MSBuild targets were not found."
    Write-Log "Expected one of:"
    Write-Log "  C:\Program Files (x86)\MSBuild\Microsoft\XNA Game Studio\Microsoft.Xna.GameStudio.targets"
    Write-Log "  C:\Program Files\MSBuild\Microsoft\XNA Game Studio\Microsoft.Xna.GameStudio.targets"
    Write-Log ""
    Write-Log "This machine may have XNA runtime/redist files, but not the developer build targets required for an Xbox 360 XNA project."
    Write-Log "No placeholder assemblies or fake references were used."
    exit 2
}

$msbuildCandidates = @(
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2010\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2010\Express\MSBuild\Current\Bin\MSBuild.exe",
    "${env:WINDIR}\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe",
    "${env:WINDIR}\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"
) | Where-Object { $_ -and (Test-Path $_) }

if ($msbuildCandidates.Count -eq 0) {
    Write-Log "BLOCKER: MSBuild 4.0 / Visual Studio 2010 MSBuild was not found."
    Write-Log "XNA 4.0 Xbox 360 projects require the legacy build toolchain."
    exit 3
}

$msbuild = $msbuildCandidates[0]
Write-Log "Using XNA targets: $($xnaTargets[0])"
Write-Log "Using MSBuild: $msbuild"
Write-Log ""

& $msbuild $projectPath /p:Configuration=$Configuration "/p:Platform=$Platform" /v:m 2>&1 |
    ForEach-Object { Write-Log $_.ToString() }

exit $LASTEXITCODE
