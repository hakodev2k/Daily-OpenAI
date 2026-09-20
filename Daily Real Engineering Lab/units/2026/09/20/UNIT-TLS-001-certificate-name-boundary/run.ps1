$ErrorActionPreference = 'Stop'
$client = Get-Content "$PSScriptRoot/starter/client.json" -Raw | ConvertFrom-Json
$cert = Get-Content "$PSScriptRoot/starter/certificate.json" -Raw | ConvertFrom-Json
$uri = [Uri]$client.endpoint
$hostName = $uri.Host
$nameMatches = $cert.dnsNames -contains $hostName
Write-Host "TCP reachable: $($client.tcpReachable)"
Write-Host "Request host: $hostName"
Write-Host "Certificate DNS names: $($cert.dnsNames -join ', ')"
Write-Host "Certificate trusted: $($cert.trusted)"
if (-not $client.tcpReachable) { Write-Host 'RESULT=NETWORK_FAILURE'; exit 10 }
if (-not $cert.valid -or -not $cert.trusted) { Write-Host 'RESULT=CERTIFICATE_VALIDATION_FAILURE'; exit 11 }
if (-not $nameMatches) { Write-Host 'RESULT=IDENTITY_VALIDATION_FAILURE'; exit 12 }
Write-Host 'RESULT=HTTPS_REQUEST_CAN_PROCEED'; exit 0