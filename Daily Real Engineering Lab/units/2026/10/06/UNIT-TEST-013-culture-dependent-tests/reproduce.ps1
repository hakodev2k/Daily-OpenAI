$ErrorActionPreference="Stop"
$root=Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet build "$root/starter/CultureLab.csproj" --nologo
if($LASTEXITCODE -ne 0){throw "Build failed."}
$us=(& dotnet run --project "$root/starter/CultureLab.csproj" --no-build -- en-US 2>&1 | Out-String); $usCode=$LASTEXITCODE
$sv=(& dotnet run --project "$root/starter/CultureLab.csproj" --no-build -- sv-SE 2>&1 | Out-String); $svCode=$LASTEXITCODE
Write-Host $us
Write-Host $sv
if($usCode -eq 0 -and $svCode -eq 0){throw "Starter unexpectedly satisfies the contract in both environments."}
if($us -notmatch "CULTURE=en-US" -or $sv -notmatch "CULTURE=sv-SE"){throw "Culture evidence missing."}
Write-Host "REPRODUCE=PASS"
