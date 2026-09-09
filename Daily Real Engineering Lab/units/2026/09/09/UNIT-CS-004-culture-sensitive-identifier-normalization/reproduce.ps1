$ErrorActionPreference = "Stop"
$output = dotnet run --project (Join-Path $PSScriptRoot "starter/Starter.csproj") 2>&1
$output | ForEach-Object { Write-Host $_ }
$text = $output -join "`n"
if ($text -notmatch "CULTURE=tr-TR" -or $text -notmatch "LOOKUP=MISS") {
    throw "Expected culture-sensitive lookup failure was not reproduced."
}
Write-Host "REPRODUCED"
