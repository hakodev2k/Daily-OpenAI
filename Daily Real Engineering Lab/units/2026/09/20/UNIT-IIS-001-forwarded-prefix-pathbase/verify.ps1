$ErrorActionPreference = 'Stop'
dotnet build ./starter/IisPrefixLab.csproj
$out = dotnet run --project ./starter -- verify 2>&1 | Out-String
$out
if ($LASTEXITCODE -ne 0 -or $out -notmatch 'VERIFY_PASS') { throw 'Learner-editable starter does not yet preserve the public proxy prefix.' }
Write-Host 'VERIFICATION_PASS'