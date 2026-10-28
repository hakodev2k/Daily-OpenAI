$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
Push-Location $PSScriptRoot
try {
    docker compose up -d --wait
    if ($LASTEXITCODE -ne 0) { throw 'Docker Compose startup failed.' }

    Get-Content -Raw -Encoding UTF8 './starter/01-schema.sql' | docker compose exec -T db psql -X -v ON_ERROR_STOP=1 -U lab -d lab
    if ($LASTEXITCODE -ne 0) { throw 'Learner schema setup failed.' }

    Get-Content -Raw -Encoding UTF8 './starter/03-verify.sql' | docker compose exec -T db psql -X -v ON_ERROR_STOP=1 -U lab -d lab
    if ($LASTEXITCODE -ne 0) { throw 'Verification failed: the learner-editable starter does not yet satisfy the slot invariant.' }

    Write-Host 'PASS: learner-edited schema rejects conflicts and accepts legal slots.'
}
finally { Pop-Location }
