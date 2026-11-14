$ErrorActionPreference='Stop'
$p=Join-Path $PSScriptRoot '../starter/Dispatch.csproj'
dotnet run --project $p -- --verify
if($LASTEXITCODE -ne 0){Write-Error 'Learner starter chưa đáp ứng contract';exit 1}
Write-Host 'VERIFY PASS'
exit 0
