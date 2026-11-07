$ErrorActionPreference='Stop'
$project=Join-Path $PSScriptRoot 'starter/ResponseLab.csproj'
$job=Start-Process dotnet -ArgumentList @('run','--project',$project,'--urls','http://127.0.0.1:5191') -PassThru -WindowStyle Hidden
try {
 Start-Sleep -Seconds 2
 try { $r=Invoke-WebRequest 'http://127.0.0.1:5191/reports/problem' -ErrorAction Stop; $status=$r.StatusCode; $body=$r.Content }
 catch { if($_.Exception.Response){$status=[int]$_.Exception.Response.StatusCode; $body=''} else {$status='connection-ended';$body=''} }
 Write-Host "status=$status body=$body"
 if($status -eq 500 -and $body -match 'report-generation-failed'){throw 'Starter did not reproduce the intended lifecycle symptom.'}
 Write-Host 'PASS: failure occurred without the expected clean JSON 500 contract.'
} finally { if(!$job.HasExited){Stop-Process -Id $job.Id -Force} }
