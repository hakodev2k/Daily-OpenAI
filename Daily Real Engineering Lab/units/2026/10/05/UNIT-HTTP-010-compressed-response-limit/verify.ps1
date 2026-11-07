$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet build "$root/starter/Client/Client.csproj"
if ($LASTEXITCODE -ne 0) { throw "Learner code does not build." }

$output = (& dotnet run --no-build --project "$root/starter/Client/Client.csproj" 2>&1 | Out-String)
$exitCode = $LASTEXITCODE
Write-Host $output

if ($exitCode -eq 0) {
    throw "Expected learner code to reject the oversized consumed payload."
}
if ($output -notmatch "(?i)(exceed|limit|too large|oversized|1,000,000|1000000)") {
    throw "Client failed, but output does not show that the size policy rejected the payload."
}
if ($output -match "Bytes materialized in memory:\s*8000") {
    throw "Client still materialized the oversized payload before rejecting it."
}
Write-Host "VERIFY=PASS"
