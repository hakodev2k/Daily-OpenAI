$ErrorActionPreference='Stop'; dotnet run --project (Join-Path $PSScriptRoot '../starter/VaccineQueue.csproj') -- --verify; exit $LASTEXITCODE
