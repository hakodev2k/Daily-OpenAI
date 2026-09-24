$ErrorActionPreference='Stop'
$project=Join-Path $PSScriptRoot 'starter/ResponseLab.csproj'
dotnet build $project -nologo | Out-Host
$job=Start-Process dotnet -ArgumentList @('run','--no-build','--project',$project,'--urls','http://127.0.0.1:5191') -PassThru -WindowStyle Hidden
try {
 Start-Sleep -Seconds 2
 $ok=Invoke-WebRequest 'http://127.0.0.1:5191/reports/good'
 if($ok.StatusCode -ne 200 -or $ok.Content -notmatch 'A-100,42'){throw 'Success path regressed.'}
 try { Invoke-WebRequest 'http://127.0.0.1:5191/reports/problem' -ErrorAction Stop | Out-Null; throw 'Failure path unexpectedly returned success.' }
 catch {
   if(!$_.Exception.Response){throw}
   $status=[int]$_.Exception.Response.StatusCode
   if($status -ne 500){throw "Expected 500 for preparation failure, got $status"}
 }
 Write-Host 'PASS: success path is complete and failure path commits a clear error status.'
} finally { if(!$job.HasExited){Stop-Process -Id $job.Id -Force} }
