$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/Starter.csproj
}
finally {
    Pop-Location
}
