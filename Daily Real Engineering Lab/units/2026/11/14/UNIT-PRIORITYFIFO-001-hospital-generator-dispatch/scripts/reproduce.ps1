$ErrorActionPreference='Stop'
$p=Join-Path $PSScriptRoot '../starter/Dispatch.csproj'
dotnet run --project $p -- --reproduce
if($LASTEXITCODE -ne 0){Write-Error 'Không reproduce được starter';exit 1}
Write-Host 'REPRODUCED'
exit 0
