$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
  dotnet build ./starter/WeatherImport.csproj -c Release --nologo
  if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
  $outputs = @()
  foreach ($culture in @('en-US','fr-FR','de-DE')) {
    $out = @(dotnet run --no-build -c Release --project ./starter/WeatherImport.csproj -- $culture)
    if ($LASTEXITCODE -ne 0) { throw "Execution failed: $culture" }
    $measurements = @($out | Where-Object { $_ -match '^(ACCEPT|REJECT) ' })
    if ($measurements.Count -ne 2) { throw "Expected two records for $culture" }
    if ($measurements[0] -cne 'ACCEPT S-41 12.75' -or $measurements[1] -cne 'ACCEPT S-42 -3.50') { throw "Contract mismatch for $culture : $($measurements -join ', ')" }
    $outputs += ($measurements -join ';')
  }
  if (@($outputs | Select-Object -Unique).Count -ne 1) { throw 'Cross-culture output mismatch' }
  Write-Host 'PASS: learner starter implementation preserves protocol across three cultures'
} finally { Pop-Location }
