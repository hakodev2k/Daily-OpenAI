$ErrorActionPreference='Stop'; dotnet run --project (Join-Path $PSScriptRoot '../starter/VaccineQueue.csproj') -- --reproduce; exit $LASTEXITCODE
