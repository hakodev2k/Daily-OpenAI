$ErrorActionPreference='Stop'
$root=Join-Path $PSScriptRoot 'starter'
$job=Start-Process dotnet -ArgumentList @('run','--project',"$root/ProxyLab.csproj",'--urls','http://127.0.0.1:5181') -PassThru -WindowStyle Hidden
try {
  Start-Sleep -Seconds 2
  $uri='http://127.0.0.1:5181/portal'
  $status=''
  for($i=0;$i -lt 3;$i++) {
    try { $r=Invoke-WebRequest $uri -Headers @{'X-Forwarded-Proto'='https';'Host'='portal.example.test'} -MaximumRedirection 0 -ErrorAction Stop; $status=$r.StatusCode; break }
    catch { if($_.Exception.Response){$status=[int]$_.Exception.Response.StatusCode; $loc=$_.Exception.Response.Headers.Location; Write-Host "hop=$i status=$status location=$loc"} else {throw} }
  }
  if($status -eq 200){ throw 'Starter did not reproduce the expected redirect symptom.' }
  Write-Host 'PASS: starter reproduced the public-request redirect symptom.'
} finally { if(!$job.HasExited){Stop-Process -Id $job.Id -Force} }
