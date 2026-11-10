$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/ArchiveLab.csproj')
exit $LASTEXITCODE
