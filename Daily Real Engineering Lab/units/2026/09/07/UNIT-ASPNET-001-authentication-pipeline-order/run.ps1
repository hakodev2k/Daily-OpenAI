$ErrorActionPreference = 'Stop'
$root = Join-Path $PSScriptRoot 'starter'
Push-Location $root
try {
    dotnet restore
    dotnet run --urls http://localhost:5088
}
finally {
    Pop-Location
}
