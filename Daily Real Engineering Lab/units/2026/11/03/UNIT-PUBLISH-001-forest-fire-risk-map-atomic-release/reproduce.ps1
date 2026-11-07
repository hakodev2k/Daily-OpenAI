$ErrorActionPreference = 'Stop'
$output = dotnet run --project ./starter/FireMap.csproj
$output | ForEach-Object { Write-Output $_ }
if ($LASTEXITCODE -ne 0) { throw 'Starter failed to run' }
if (-not ($output -match 'consistent=False')) { throw 'Expected reproducible mixed-release snapshot' }
Write-Output 'REPRODUCED: mixed release snapshot'
