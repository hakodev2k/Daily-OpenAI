$ErrorActionPreference='Stop'
$root=Join-Path $PSScriptRoot '..'
dotnet run --project (Join-Path $root 'starter/BatchAudit.csproj')
exit $LASTEXITCODE
