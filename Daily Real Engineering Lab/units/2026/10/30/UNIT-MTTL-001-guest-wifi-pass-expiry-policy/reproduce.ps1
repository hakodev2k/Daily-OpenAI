$ErrorActionPreference = "Stop"
$unitRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $unitRoot
try {
    & dotnet run --project (Join-Path $unitRoot "starter/VisitorPassLab.csproj") --configuration Release -- --reproduce
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally {
    Pop-Location
}
