$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/RedisOutageLab.csproj" -- --reproduce
$code = $LASTEXITCODE
if ($code -eq 0) { throw 'Expected the original starter to expose cache-outage request failures.' }
if ($code -ne 1) { throw "Starter did not reproduce the expected controlled symptom. Exit code: $code" }
Write-Host 'Reproduction confirmed: a short cache outage causes request failures while the origin remains available.'
