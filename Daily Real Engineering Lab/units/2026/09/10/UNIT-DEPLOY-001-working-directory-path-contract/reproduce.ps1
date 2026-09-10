$ErrorActionPreference='Stop'
$project = Join-Path $PSScriptRoot 'starter/Starter.csproj'
Push-Location $PSScriptRoot
try {
  dotnet build $project -nologo | Out-Null
  $dll = Join-Path $PSScriptRoot 'starter/bin/Debug/net8.0/Starter.dll'
  $output = dotnet $dll 2>&1 | Out-String
  $output
  if ($LASTEXITCODE -ne 2 -or $output -notmatch 'Template not found') { throw 'Expected deployment-context failure was not reproduced.' }
  Write-Host 'LAB_REPRODUCE_PASS'
} finally { Pop-Location }
