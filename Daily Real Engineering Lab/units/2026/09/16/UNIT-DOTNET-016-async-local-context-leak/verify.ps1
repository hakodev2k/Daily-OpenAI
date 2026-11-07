$ErrorActionPreference='Stop'
$root=Join-Path $PSScriptRoot 'starter'
dotnet run --project $root -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Verification failed. Detached work still observes request correlation context.' }
Write-Host 'Verification passed.'