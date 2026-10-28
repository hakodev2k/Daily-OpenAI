$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet run --project ./starter/GrantExport.csproj -- demo
    $result = $LASTEXITCODE
} finally { Pop-Location }
if ($result -ne 0) { throw "demo failed (exit $result)." }
Write-Host "PASS: demo completed."
