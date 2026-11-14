$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/ClinicChannel.csproj'
& dotnet run --project $project --no-launch-profile
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host 'Reproduce: quan sát ERROR worker và RESULT processed=1 errors=1.'
