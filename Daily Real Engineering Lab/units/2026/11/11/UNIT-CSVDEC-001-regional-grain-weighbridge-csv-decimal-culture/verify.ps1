$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Weighbridge.csproj'
foreach ($culture in @('en-US','de-DE','vi-VN','fr-FR')) {
  $output = @(dotnet run --project $project -- $culture 2>&1)
  if ($LASTEXITCODE -ne 0 -or (($output -join ' ') -notmatch 'kilograms=1250\.75(?:\s|$)')) { throw "VERIFY FAILED for $culture : $($output -join ' ')" }
}
Write-Host 'VERIFY PASSED: invariant partner payload is identical across cultures.'
