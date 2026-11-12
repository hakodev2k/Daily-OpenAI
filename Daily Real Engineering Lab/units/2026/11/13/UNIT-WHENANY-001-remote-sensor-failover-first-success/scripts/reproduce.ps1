$ErrorActionPreference='Stop'
$output= & dotnet run --project (Join-Path $PSScriptRoot '../starter/SensorFailover.csproj') --configuration Release -- 2>&1 | Out-String
$code=$LASTEXITCODE
Write-Output $output
if($code -ne 0 -and $output.Contains('UNAVAILABLE:primary:unavailable')) {Write-Output 'REPRODUCED';exit 0}
Write-Error 'Original failure not reproduced';exit 1
