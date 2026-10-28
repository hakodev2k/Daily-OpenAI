$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet run --project ./starter/GrantExport.csproj -- verify
    $result = $LASTEXITCODE
} finally { Pop-Location }
if ($result -ne 0) { throw "verify failed (exit $result)." }
Write-Host "PASS: verify completed."
