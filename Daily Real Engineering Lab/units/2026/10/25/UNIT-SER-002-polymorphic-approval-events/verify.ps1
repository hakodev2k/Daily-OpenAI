$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project starter/Lab.Starter.csproj -- verify
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally { Pop-Location }
