$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/IntakeLab.csproj') -- reproduce
exit $LASTEXITCODE
