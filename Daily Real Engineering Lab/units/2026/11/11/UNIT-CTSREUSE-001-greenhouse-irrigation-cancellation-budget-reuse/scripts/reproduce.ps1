$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/Greenhouse.csproj'
$output = @(dotnet run --project $project 2>&1)
if ($LASTEXITCODE -ne 0) { $output; exit 2 }
$output
if (($output -match 'cycle=1 status=ok').Count -eq 1 -and ($output -match 'cycle=2 status=cancelled').Count -eq 1) {
  Write-Host 'REPRODUCED: second cycle cancelled'
  exit 0
}
throw 'Expected baseline symptom not reproduced. Restore original starter or inspect environment.'
