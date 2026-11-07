$ErrorActionPreference = "Stop"
$unitRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $unitRoot
try {
& dotnet run --project (Join-Path $unitRoot "starter/LaundryAudit/LaundryAudit.csproj") --configuration Release
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally {
    Pop-Location
}
