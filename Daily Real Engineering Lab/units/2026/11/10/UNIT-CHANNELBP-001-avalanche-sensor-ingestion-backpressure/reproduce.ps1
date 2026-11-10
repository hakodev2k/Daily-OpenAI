$ErrorActionPreference='Stop'
$project=Join-Path $PSScriptRoot 'starter/Lab.csproj'
dotnet run --project $project --no-launch-profile
$code=$LASTEXITCODE
if ($code -eq 1) { Write-Host 'REPRODUCED: expected starter data loss'; exit 0 }
throw "Unexpected starter exit code: $code (expected 1)"
