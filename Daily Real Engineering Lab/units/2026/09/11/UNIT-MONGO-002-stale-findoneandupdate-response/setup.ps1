$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    docker compose up -d
    dotnet restore ./starter/Lab.csproj
    Write-Host 'MongoDB started on localhost:27018 and starter dependencies restored.'
}
finally {
    Pop-Location
}
