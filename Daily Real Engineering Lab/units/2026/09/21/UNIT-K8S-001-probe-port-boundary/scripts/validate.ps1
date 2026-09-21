param([Parameter(Mandatory=$true)][string]$Deployment)
$ErrorActionPreference = 'Stop'
$text = Get-Content -Raw $Deployment
$listener = [regex]::Match($text, 'ASPNETCORE_URLS[\s\S]*?value:\s*http://0\.0\.0\.0:(\d+)').Groups[1].Value
$probeBlock = [regex]::Match($text, 'readinessProbe:[\s\S]*?httpGet:[\s\S]*?port:\s*(\d+)')
$probe = $probeBlock.Groups[1].Value
if (-not $listener -or -not $probe) { throw 'Không đọc được listener/probe contract từ deployment.' }
Write-Host "Application listener: $listener"
Write-Host "Readiness probe port: $probe"
if ($listener -eq $probe) { Write-Host 'READY_CONTRACT_OK'; exit 0 }
Write-Host 'READY_CONTRACT_BROKEN'; exit 2
