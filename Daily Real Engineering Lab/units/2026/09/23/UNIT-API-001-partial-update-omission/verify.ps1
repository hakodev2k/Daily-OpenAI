$ErrorActionPreference = "Stop"
$output = dotnet run --project "$PSScriptRoot/starter/ApiContractLab.csproj"
$output | Write-Host
if ($output -notmatch "After : DisplayName=Merchant Alpha, EmailEnabled=True") { throw "Verification failed: an omitted setting was not preserved." }
Write-Host "Primary scenario passed. Also verify explicit false and record it in workspace/my-investigation.md."
