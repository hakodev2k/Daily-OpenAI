param(
    [ValidateSet("reproduce", "verify")]
    [string]$Mode = "reproduce"
)
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    dotnet run --project "./starter/WaveformRangeLab.csproj" -- $Mode
    if ($LASTEXITCODE -ne 0) { throw "Lab $Mode failed with exit code $LASTEXITCODE" }
} finally {
    Pop-Location
}
