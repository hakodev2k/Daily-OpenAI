$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/HeritageUpload.csproj')
exit $LASTEXITCODE
