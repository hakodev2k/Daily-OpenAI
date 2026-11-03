$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = & dotnet run --project (Join-Path $root "starter/WebhookLab.csproj") 2>&1
if ($LASTEXITCODE -ne 0) { $output; exit 2 }
$text = $output -join "`n"
$output
if ($text -match "Applied=1" -and $text -match "DUPLICATE" -and $text -match "REJECT stale") { exit 0 }
Write-Error "Verification failed: require one applied event, duplicate detection and stale rejection"; exit 1
