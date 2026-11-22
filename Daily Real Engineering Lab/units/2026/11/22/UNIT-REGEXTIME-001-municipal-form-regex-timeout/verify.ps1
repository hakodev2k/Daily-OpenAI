$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/FormValidator.csproj'
dotnet build $project --configuration Release
if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
$output = dotnet run --no-build --configuration Release --project $project -- verify
if ($LASTEXITCODE -ne 0) { throw "Adversarial verification failed: $output" }
if ($output -notmatch 'accepted=false') { throw "Unexpected adversarial result: $output" }
$normal = dotnet run --no-build --configuration Release --project $project -- normal
if ($LASTEXITCODE -ne 0 -or $normal -notmatch 'accepted=True|accepted=true') { throw "Valid input rejected: $normal" }
Write-Host "PASS: modified starter accepts valid input and rejects near-match within 2000 ms."
