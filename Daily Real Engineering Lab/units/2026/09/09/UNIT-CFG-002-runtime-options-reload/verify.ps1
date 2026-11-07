$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/RuntimeOptionsReload.csproj"
$output | ForEach-Object { Write-Host $_ }

if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$before = $output | Where-Object { $_ -eq 'REQUEST_BEFORE=https://primary.internal' }
$configAfter = $output | Where-Object { $_ -eq 'CONFIG_AFTER=https://failover.internal' }
$requestAfter = $output | Where-Object { $_ -eq 'REQUEST_AFTER=https://failover.internal' }

if (-not $before -or -not $configAfter -or -not $requestAfter) {
    Write-Error 'Verification failed: learner-editable starter does not observe the updated endpoint after reload.'
    exit 1
}

Write-Host 'VERIFIED: runtime reload is observed without process restart.'
