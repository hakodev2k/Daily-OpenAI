$ErrorActionPreference='Stop'
$root=Join-Path $PSScriptRoot '..'
dotnet restore (Join-Path $root 'starter/BatchAudit.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
