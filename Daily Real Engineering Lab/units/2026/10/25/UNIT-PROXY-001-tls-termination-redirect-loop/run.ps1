param([int]$Port = 5077)
$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    & dotnet run --project starter/ProxyLab.Starter.csproj --no-launch-profile -- --urls "http://127.0.0.1:$Port"
    if ($LASTEXITCODE -ne 0) { throw "Server exited with code $LASTEXITCODE" }
}
finally { Pop-Location }
