$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/HarborFrame.csproj'
foreach ($case in @('fragmented','single','truncated','empty')) {
  Write-Host "CASE $case"
  dotnet run --project $project -- $case
  if ($LASTEXITCODE -ne 0) { Write-Error "FAIL: $case"; exit 1 }
}
Write-Host 'PASS: learner workspace meets frame contract'
