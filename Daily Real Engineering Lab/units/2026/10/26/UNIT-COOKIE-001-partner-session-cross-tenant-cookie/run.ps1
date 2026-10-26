param([ValidateSet("reproduce", "verify")][string]$Mode = "reproduce")
$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    dotnet run --project "./starter/PartnerCookieLab.csproj" -- $Mode
    exit $LASTEXITCODE
} finally { Pop-Location }
