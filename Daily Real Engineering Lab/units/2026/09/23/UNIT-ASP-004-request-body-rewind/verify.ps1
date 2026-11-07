$ErrorActionPreference='Stop'
dotnet build ./starter/WebhookLab.csproj --nologo | Out-Null
$body='{"paymentId":"pay-42","status":"captured"}'
$key=[Text.Encoding]::UTF8.GetBytes('lab-secret')
$hmac=[Security.Cryptography.HMACSHA256]::new($key)
try { $sig=[Convert]::ToHexString($hmac.ComputeHash([Text.Encoding]::UTF8.GetBytes($body))) } finally { $hmac.Dispose() }
$response=Invoke-RestMethod -Method Post -Uri 'http://localhost:5084/webhooks/payment' -ContentType 'application/json' -Headers @{'X-Lab-Signature'=$sig} -Body $body
if (-not $response.accepted) { throw 'Webhook was not accepted.' }
Write-Host 'PASS: learner-editable starter accepts the correctly signed webhook. Confirm console AUDIT bytes is greater than zero.'