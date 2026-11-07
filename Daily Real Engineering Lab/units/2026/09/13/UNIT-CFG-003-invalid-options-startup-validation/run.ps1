$ErrorActionPreference = 'Stop'
$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $Root
try {
    dotnet run --project ./starter/Starter.csproj --no-launch-profile
}
finally {
    Pop-Location
}