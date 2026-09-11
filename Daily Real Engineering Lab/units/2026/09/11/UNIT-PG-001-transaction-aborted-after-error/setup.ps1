$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    docker compose up -d postgres

    $deadline = (Get-Date).AddSeconds(45)
    do {
        $status = docker compose ps --format json postgres 2>$null
        if ($LASTEXITCODE -eq 0 -and $status -match 'healthy') {
            break
        }
        Start-Sleep -Seconds 2
    } while ((Get-Date) -lt $deadline)

    docker compose exec -T postgres pg_isready -U lab -d engineering_lab | Out-Host
    if ($LASTEXITCODE -ne 0) {
        throw 'PostgreSQL did not become ready.'
    }

    dotnet restore ./starter/Starter.csproj
    if ($LASTEXITCODE -ne 0) {
        throw 'dotnet restore failed.'
    }

    Write-Host 'Setup complete.'
}
finally {
    Pop-Location
}
