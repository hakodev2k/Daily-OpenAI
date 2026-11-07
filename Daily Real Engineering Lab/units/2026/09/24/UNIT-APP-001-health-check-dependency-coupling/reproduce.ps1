$ErrorActionPreference = 'Stop'
$base = 'http://localhost:5088'
Invoke-RestMethod -Method Post "$base/lab/dependency/up" | Out-Null
$healthy = Invoke-WebRequest "$base/health" -SkipHttpErrorCheck
Write-Host "Health before outage: $($healthy.StatusCode)"
Invoke-RestMethod -Method Post "$base/lab/dependency/down" | Out-Null
$profile = Invoke-WebRequest "$base/api/profile/42" -SkipHttpErrorCheck
$recommendations = Invoke-WebRequest "$base/api/profile/42/recommendations" -SkipHttpErrorCheck
$health = Invoke-WebRequest "$base/health" -SkipHttpErrorCheck
Write-Host "Core profile endpoint: $($profile.StatusCode)"
Write-Host "Recommendation capability: $($recommendations.StatusCode)"
Write-Host "Instance health signal: $($health.StatusCode)"
if ($profile.StatusCode -eq 200 -and $recommendations.StatusCode -eq 503 -and $health.StatusCode -eq 503) {
  Write-Host 'REPRODUCED: core capability remains available while instance health fails.'
  exit 0
}
throw 'Expected incident symptom was not reproduced.'
