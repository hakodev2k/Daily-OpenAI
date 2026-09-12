$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet restore ./starter/Starter.csproj
    dotnet run --project ./starter/Starter.csproj
}
finally {
    Pop-Location
}
