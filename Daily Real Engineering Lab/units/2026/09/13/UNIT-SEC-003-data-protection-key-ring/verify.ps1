$ErrorActionPreference='Stop'
$project=Join-Path $PSScriptRoot 'starter/DataProtectionLab.csproj'
dotnet run --project $project
if($LASTEXITCODE -ne 0){throw 'Verification failed: learner-modified starter still cannot read protected state across instances.'}
Write-Host 'Verification passed.'
