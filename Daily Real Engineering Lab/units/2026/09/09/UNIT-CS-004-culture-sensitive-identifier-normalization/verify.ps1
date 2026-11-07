$ErrorActionPreference = "Stop"
$output = dotnet run --project (Join-Path $PSScriptRoot "starter/Starter.csproj") 2>&1
$output | ForEach-Object { Write-Host $_ }
$text = $output -join "`n"
if ($text -notmatch "LOOKUP=HIT:manual.pdf") {
    throw "Identifier lookup is still culture-sensitive or otherwise incorrect."
}
Write-Host "PASS"
