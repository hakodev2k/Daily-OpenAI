$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/FisheryPermitLab.csproj') -- reproduce
exit $LASTEXITCODE
