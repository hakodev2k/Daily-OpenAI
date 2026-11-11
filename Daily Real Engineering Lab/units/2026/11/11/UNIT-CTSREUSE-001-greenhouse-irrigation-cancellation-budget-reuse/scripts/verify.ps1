$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/Greenhouse.csproj'
$output = @(dotnet run --project $project 2>&1)
if ($LASTEXITCODE -ne 0) { $output; exit 2 }
$output
if (($output -match 'cycle=1 status=ok').Count -ne 1 -or ($output -match 'cycle=2 status=ok').Count -ne 1 -or ($output -match 'status=cancelled').Count -gt 0) {
  throw 'VERIFY FAILED: both learner-edited cycles must succeed without cancellation.'
}
Write-Host 'VERIFY PASS: both cycles completed'
