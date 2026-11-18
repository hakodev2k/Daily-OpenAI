$ErrorActionPreference = 'Stop'
$out = (& dotnet run --project (Join-Path $PSScriptRoot '../starter/MuseumAudit.csproj') 2>&1 | Out-String)
if ($LASTEXITCODE -ne 0) { throw $out }
if ($out -notmatch 'Accepted=3 Persisted=2' -or $out -notmatch 'Ids=102,103') { throw "Unexpected reproduction: $out" }
Write-Output 'REPRODUCED: producer acknowledged 3, consumer received 2'
