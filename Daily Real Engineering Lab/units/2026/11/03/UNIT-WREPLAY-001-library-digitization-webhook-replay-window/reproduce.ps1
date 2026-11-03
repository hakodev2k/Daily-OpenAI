$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = & dotnet run --project (Join-Path $root "starter/WebhookLab.csproj") 2>&1
if ($LASTEXITCODE -ne 0) { $output; exit 2 }
$output
if (($output -join "`n") -match "Applied=3") { Write-Error "Reproduced: three side effects for duplicate/stale deliveries"; exit 1 }
exit 0
