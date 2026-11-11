$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
dotnet --version
dotnet restore (Join-Path $root 'starter/Triage.csproj')
if($LASTEXITCODE -ne 0){throw 'Restore failed'}
