$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/RuntimeOptionsReload.csproj"
$output | ForEach-Object { Write-Host $_ }

if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$configAfter = $output | Where-Object { $_ -eq 'CONFIG_AFTER=https://failover.internal' }
$requestAfter = $output | Where-Object { $_ -eq 'REQUEST_AFTER=https://primary.internal' }

if (-not $configAfter -or -not $requestAfter) {
    Write-Error 'Starter did not reproduce the intended runtime configuration symptom.'
    exit 1
}

Write-Host 'REPRODUCED: configuration changed, but the service still uses the previous endpoint.'
