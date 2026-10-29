$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    dotnet restore "./checks/Checks.csproj"
    $code = $LASTEXITCODE
}
finally { Pop-Location }
exit $code
