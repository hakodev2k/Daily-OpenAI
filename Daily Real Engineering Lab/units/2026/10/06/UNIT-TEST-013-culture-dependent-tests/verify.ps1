$ErrorActionPreference="Stop"
$root=Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet build "$root/starter/CultureLab.csproj" --nologo
if($LASTEXITCODE -ne 0){throw "Build failed."}
foreach($culture in @("en-US","sv-SE")){
  $output=(& dotnet run --project "$root/starter/CultureLab.csproj" --no-build -- $culture 2>&1 | Out-String)
  $code=$LASTEXITCODE
  Write-Host $output
  if($code -ne 0){throw "Contract failed for $culture."}
  if($output -notmatch "ACTUAL=2026-10-05"){throw "Unexpected output for $culture."}
}
Write-Host "VERIFY=PASS"
