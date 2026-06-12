param(
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"
$xboxRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $xboxRoot
$logDir = Join-Path $xboxRoot "logs"
$logPath = Join-Path $logDir "desktop-build.txt"

New-Item -ItemType Directory -Force -Path $logDir | Out-Null
Set-Location $repoRoot

function Write-Log($message) {
    $message | Add-Content -Path $logPath -Encoding UTF8
    Write-Host $message
}

$commandText = "dotnet build --no-restore /p:EnableMGCBItems=false /p:Configuration=$Configuration"
"Desktop audit build" | Set-Content -Path $logPath -Encoding UTF8
Write-Log "Command: $commandText"
Write-Log "WorkingDirectory: $repoRoot"
Write-Log ""

& dotnet build --no-restore /p:EnableMGCBItems=false /p:Configuration=$Configuration 2>&1 |
    ForEach-Object { Write-Log $_.ToString() }

exit $LASTEXITCODE
