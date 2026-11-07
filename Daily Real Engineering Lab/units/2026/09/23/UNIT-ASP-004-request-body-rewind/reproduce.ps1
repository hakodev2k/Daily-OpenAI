$ErrorActionPreference='Stop'
$body='{"paymentId":"pay-42","status":"captured"}'
$key=[Text.Encoding]::UTF8.GetBytes('lab-secret')
$hmac=[Security.Cryptography.HMACSHA256]::new($key)
try { $sig=[Convert]::ToHexString($hmac.ComputeHash([Text.Encoding]::UTF8.GetBytes($body))) } finally { $hmac.Dispose() }
try { Invoke-WebRequest -Method Post -Uri 'http://localhost:5084/webhooks/payment' -ContentType 'application/json' -Headers @{'X-Lab-Signature'=$sig} -Body $body | Out-Null; throw 'Expected starter request to be rejected.' } catch { if ($_.Exception.Response.StatusCode.value__ -ne 400) { throw } }
Write-Host 'Reproduced: valid signed webhook is rejected after passing through the pipeline.'