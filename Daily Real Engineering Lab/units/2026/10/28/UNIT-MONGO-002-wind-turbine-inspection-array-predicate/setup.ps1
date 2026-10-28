$ErrorActionPreference = 'Stop'
$compose = Join-Path $PSScriptRoot 'compose.yaml'
& docker compose -f $compose up -d --wait mongo
if ($LASTEXITCODE -ne 0) { throw "MongoDB setup failed (exit $LASTEXITCODE)." }
Write-Host "MongoDB lab container is ready."
