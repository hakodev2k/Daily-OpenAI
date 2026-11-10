$ErrorActionPreference='Stop'
dotnet run --project "$PSScriptRoot/starter/MuseumRegistry.csproj" -- --contract
if($LASTEXITCODE -eq 1){Write-Host "EXPECTED_FAILURE_REPRODUCED";exit 0}
Write-Error "Expected contract failure with exit code 1; got $LASTEXITCODE"
exit 1
