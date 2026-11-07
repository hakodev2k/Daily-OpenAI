$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = & dotnet run --project "$root/starter/BulkInspector.csproj" 2>&1
if ($LASTEXITCODE -ne 0) { throw "Starter did not reproduce the intended false-success state.`n$output" }
if (($output -join "`n") -notmatch 'BATCH_OK') { throw "Expected starter to report BATCH_OK.`n$output" }
Write-Host "Reproduced: application reports batch success for the supplied evidence fixture."
