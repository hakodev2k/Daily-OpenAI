$ErrorActionPreference = 'Stop'
$output = @(dotnet run --project "$PSScriptRoot/starter/AuditLab.csproj" 2>&1)
if ($LASTEXITCODE -ne 0) { $output; exit 2 }
$output
if (($output -join "`n") -notmatch 'audit count=3 sequences=6,7,8' -or ($output -join "`n") -notmatch 'audit complete=False') { throw 'Starter symptom was not reproduced' }
Write-Host 'REPRODUCED: missing audit sequences'
