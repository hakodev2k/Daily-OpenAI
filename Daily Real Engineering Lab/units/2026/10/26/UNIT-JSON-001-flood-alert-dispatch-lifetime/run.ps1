$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet run --project './starter/FloodAlertLab.csproj' -- --observe
    if ($LASTEXITCODE -ne 0) { throw "Lab run failed: exit $LASTEXITCODE" }
}
finally { Pop-Location }
