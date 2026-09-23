$ErrorActionPreference = "Stop"
$omitted = dotnet run --project "$PSScriptRoot/starter/ApiContractLab.csproj"
$explicitFalse = dotnet run --project "$PSScriptRoot/starter/ApiContractLab.csproj" -- --explicit-false
$omitted | Write-Host
$explicitFalse | Write-Host
if ($omitted -notmatch "After : DisplayName=Merchant Alpha, EmailEnabled=True") { throw "Verification failed: omitted EmailEnabled was not preserved." }
if ($explicitFalse -notmatch "After : DisplayName=Merchant A, EmailEnabled=False") { throw "Verification failed: explicit false was not applied." }
Write-Host "VERIFIED: omission and explicit false have distinct behavior."
