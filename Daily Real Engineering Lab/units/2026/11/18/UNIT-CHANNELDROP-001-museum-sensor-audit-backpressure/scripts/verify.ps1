$ErrorActionPreference = 'Stop'
$out = (& dotnet run --project (Join-Path $PSScriptRoot '../starter/MuseumAudit.csproj') 2>&1 | Out-String)
if ($LASTEXITCODE -ne 0) { throw $out }
if ($out -notmatch 'Accepted=3 Persisted=3' -or $out -notmatch 'Ids=101,102,103') { throw "VERIFY FAILED: $out" }
Write-Output 'VERIFY PASS: all accepted events delivered in order'
