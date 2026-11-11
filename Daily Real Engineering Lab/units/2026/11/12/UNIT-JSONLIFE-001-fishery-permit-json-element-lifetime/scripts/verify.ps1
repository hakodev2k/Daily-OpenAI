$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/FisheryPermitLab.csproj') -- verify
exit $LASTEXITCODE
