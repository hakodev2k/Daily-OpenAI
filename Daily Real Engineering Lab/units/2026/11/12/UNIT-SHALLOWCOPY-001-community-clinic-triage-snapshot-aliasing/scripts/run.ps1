$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
dotnet run --project (Join-Path $root 'starter/Triage.csproj')
if($LASTEXITCODE -ne 0){throw 'Run failed'}
