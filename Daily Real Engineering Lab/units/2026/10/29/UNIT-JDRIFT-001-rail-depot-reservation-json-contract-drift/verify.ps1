$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    dotnet run --project "./checks/Checks.csproj" -- --verify
    $code = $LASTEXITCODE
}
finally { Pop-Location }
exit $code
