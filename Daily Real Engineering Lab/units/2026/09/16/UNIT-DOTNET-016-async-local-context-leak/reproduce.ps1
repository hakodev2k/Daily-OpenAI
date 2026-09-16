$ErrorActionPreference='Stop'
$root=Join-Path $PSScriptRoot 'starter'
dotnet run --project $root
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host 'Reproduction complete. Expected starter observation: detached:req-42'