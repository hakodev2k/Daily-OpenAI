$ErrorActionPreference = 'Continue'
dotnet run --project ./starter/ChannelLab.csproj -- reproduce
if ($LASTEXITCODE -eq 2) { Write-Host 'Expected shutdown stall reproduced.'; exit 0 }
Write-Error 'Expected symptom was not reproduced.'
exit 1
