$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ExhibitLedger.csproj'
& dotnet build $project -c Release --nologo
if ($LASTEXITCODE -ne 0) { throw 'Build failed; cannot reproduce.' }
$dll = Join-Path $PSScriptRoot 'starter/bin/Release/net8.0/ExhibitLedger.dll'
& dotnet $dll
$code = $LASTEXITCODE
if ($code -ne 2) { throw "Expected original deadlock exit code 2; observed $code. Restore original starter if already fixed." }
Write-Host 'PASS: original concurrency failure reproduced (expected exit code 2).'
