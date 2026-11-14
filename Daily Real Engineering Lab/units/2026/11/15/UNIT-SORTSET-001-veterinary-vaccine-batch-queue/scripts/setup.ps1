$ErrorActionPreference='Stop'; dotnet restore (Join-Path $PSScriptRoot '../starter/VaccineQueue.csproj'); if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
