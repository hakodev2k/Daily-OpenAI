$ErrorActionPreference = 'Continue'
dotnet run --project ./starter/MetricLab.csproj -- reproduce
if ($LASTEXITCODE -eq 2) { Write-Host 'Expected telemetry symptom reproduced.'; exit 0 }
Write-Error 'Expected telemetry symptom was not reproduced.'
exit 1
