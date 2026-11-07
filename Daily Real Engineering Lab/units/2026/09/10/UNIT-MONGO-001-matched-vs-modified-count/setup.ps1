$ErrorActionPreference = 'Stop'

Push-Location $PSScriptRoot
try {
    docker compose up -d

    Write-Host 'Waiting for MongoDB health check...'
    $ready = $false
    for ($i = 0; $i -lt 30; $i++) {
        $status = docker inspect --format='{{.State.Health.Status}}' real-engineering-lab-mongo-001 2>$null
        if ($status -eq 'healthy') {
            $ready = $true
            break
        }
        Start-Sleep -Seconds 1
    }

    if (-not $ready) {
        throw 'MongoDB container did not become healthy. Run: docker compose logs mongo'
    }

    dotnet restore ./starter/Starter.csproj
    dotnet restore ./solution/Solution.csproj
    Write-Host 'Setup complete.'
}
finally {
    Pop-Location
}
