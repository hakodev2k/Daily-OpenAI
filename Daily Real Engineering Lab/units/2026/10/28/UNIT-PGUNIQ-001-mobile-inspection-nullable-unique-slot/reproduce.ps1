$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
Push-Location $PSScriptRoot
try {
    docker compose up -d --wait
    if ($LASTEXITCODE -ne 0) { throw 'Docker Compose startup failed.' }

    Get-Content -Raw -Encoding UTF8 './starter/01-schema.sql' | docker compose exec -T db psql -X -v ON_ERROR_STOP=1 -U lab -d lab
    if ($LASTEXITCODE -ne 0) { throw 'Starter schema setup failed.' }

    Get-Content -Raw -Encoding UTF8 './starter/02-reproduce.sql' | docker compose exec -T db psql -X -v ON_ERROR_STOP=1 -U lab -d lab
    if ($LASTEXITCODE -ne 0) { throw 'Expected reproduction evidence was not observed.' }

    Write-Host 'Reproduction confirmed. Record evidence before changing starter/01-schema.sql.'
}
finally { Pop-Location }
