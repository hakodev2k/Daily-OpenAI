$ErrorActionPreference='Stop'
dotnet run --project "$PSScriptRoot/starter/MuseumRegistry.csproj" -- --contract
if($LASTEXITCODE -ne 0){Write-Error "Contract failed";exit 1}
Write-Host "VERIFIED_STARTER_CONTRACT"
exit 0
