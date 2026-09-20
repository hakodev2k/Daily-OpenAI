$ErrorActionPreference = 'Stop'
dotnet build ./starter/IisPrefixLab.csproj
$out = dotnet run --project ./starter -- reproduce 2>&1 | Out-String
$out
if ($LASTEXITCODE -ne 0 -or $out -notmatch 'SYMPTOM_REPRODUCED') { throw 'Starter symptom was not reproduced deterministically.' }
Write-Host 'REPRODUCE_PASS'