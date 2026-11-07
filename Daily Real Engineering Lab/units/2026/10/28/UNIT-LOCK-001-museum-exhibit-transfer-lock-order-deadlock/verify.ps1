$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ExhibitLedger.csproj'
& dotnet build $project -c Release --nologo
if ($LASTEXITCODE -ne 0) { throw 'Build failed; verification stopped.' }
$dll = Join-Path $PSScriptRoot 'starter/bin/Release/net8.0/ExhibitLedger.dll'
& dotnet $dll
$code = $LASTEXITCODE
if ($code -ne 0) { throw "Verification failed: learner starter returned exit code $code." }
Write-Host 'PASS: learner-edited starter satisfies liveness and inventory invariant.'
