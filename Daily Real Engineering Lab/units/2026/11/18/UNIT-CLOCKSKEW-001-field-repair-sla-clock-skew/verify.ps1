$ErrorActionPreference='Stop'
$project=if($args.Count -gt 0){$args[0]}else{"$PSScriptRoot/starter/ClockLab.csproj"}
dotnet run --project $project -- --check
if($LASTEXITCODE -ne 0){throw 'Verification failed'}
Write-Host 'Verification passed'
