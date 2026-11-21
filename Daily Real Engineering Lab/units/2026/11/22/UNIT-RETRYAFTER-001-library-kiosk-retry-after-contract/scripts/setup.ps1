$ErrorActionPreference = 'Stop'
dotnet restore (Join-Path $PSScriptRoot '../starter/KioskBackoff.csproj')
exit $LASTEXITCODE
