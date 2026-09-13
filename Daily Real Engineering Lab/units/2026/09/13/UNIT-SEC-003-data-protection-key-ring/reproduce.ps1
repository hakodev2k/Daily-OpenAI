$ErrorActionPreference='Stop'
$project=Join-Path $PSScriptRoot 'starter/DataProtectionLab.csproj'
dotnet run --project $project
if($LASTEXITCODE -eq 0){throw 'Expected cross-instance protected-state failure, but starter passed.'}
Write-Host 'Reproduction succeeded: cross-instance read failed while self-read succeeded.'
exit 0
