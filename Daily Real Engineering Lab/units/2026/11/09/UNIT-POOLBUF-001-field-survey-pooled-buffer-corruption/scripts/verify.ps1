$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/ThumbnailLab.csproj'
for ($i = 1; $i -le 10; $i++) {
  dotnet run --project $project --configuration Release
  if ($LASTEXITCODE -ne 0) { Write-Error "Verification failed at iteration $i"; exit 1 }
}
Write-Host 'PASS: 10 successful iterations on learner starter'
exit 0
