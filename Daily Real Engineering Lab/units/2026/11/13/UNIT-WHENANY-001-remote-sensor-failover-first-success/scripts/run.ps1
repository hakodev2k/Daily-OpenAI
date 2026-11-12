$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/SensorFailover.csproj') --configuration Release --
exit $LASTEXITCODE
