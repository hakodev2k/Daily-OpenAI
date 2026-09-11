$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'
dotnet run --project $project
$code = $LASTEXITCODE
if ($code -eq 42) {
  Write-Host 'PASS: reproduced the intended starter failure.'
  exit 0
}
Write-Error "Expected starter exit code 42, got $code. If you already edited starter/, use git restore on this unit before reproducing."
exit 1
