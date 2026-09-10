$ErrorActionPreference='Stop'
$project = Join-Path $PSScriptRoot 'starter/Starter.csproj'
dotnet build $project -nologo | Out-Null
$dll = Join-Path $PSScriptRoot 'starter/bin/Debug/net8.0/Starter.dll'
Push-Location ([System.IO.Path]::GetTempPath())
try {
  $output = dotnet $dll 2>&1 | Out-String
  $output
  if ($LASTEXITCODE -ne 0) { throw 'Learner code still fails from an external working directory.' }
  if ($output -notmatch 'INVOICE=Invoice for ACME') { throw 'Expected invoice output was not preserved.' }
  Write-Host 'LAB_VERIFY_PASS'
} finally { Pop-Location }
