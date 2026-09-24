$ErrorActionPreference='Stop'
$root=Join-Path $PSScriptRoot 'starter'
dotnet build "$root/ProxyLab.csproj" -nologo | Out-Host
$job=Start-Process dotnet -ArgumentList @('run','--no-build','--project',"$root/ProxyLab.csproj",'--urls','http://127.0.0.1:5181') -PassThru -WindowStyle Hidden
try {
  Start-Sleep -Seconds 2
  $r=Invoke-WebRequest 'http://127.0.0.1:5181/portal' -Headers @{'X-Forwarded-Proto'='https';'Host'='portal.example.test'} -MaximumRedirection 0
  if($r.StatusCode -ne 200){throw "Expected 200, got $($r.StatusCode)"}
  $body=$r.Content | ConvertFrom-Json
  if($body.scheme -ne 'https'){throw "Expected backend public scheme https, got $($body.scheme)"}
  Write-Host 'PASS: learner code preserves public HTTPS semantics and returns 200.'
} finally { if(!$job.HasExited){Stop-Process -Id $job.Id -Force} }
