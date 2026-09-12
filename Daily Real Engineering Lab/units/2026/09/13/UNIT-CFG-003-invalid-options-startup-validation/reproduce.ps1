$ErrorActionPreference = 'Stop'
$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $Root
try {
    $output = (& dotnet run --project ./starter/Starter.csproj --no-launch-profile 2>&1 | Out-String)
    $exitCode = $LASTEXITCODE

    Write-Host $output

    if ($exitCode -eq 0) { throw 'Expected starter to fail after startup, but exit code was 0.' }
    if ($output -notmatch 'APP_STARTED') { throw 'Starter did not prove that startup completed before the failure.' }
    if ($output -notmatch 'REQUEST_FAILED:UriFormatException') { throw 'Expected the first business operation to fail with UriFormatException.' }

    Write-Host 'REPRODUCED: application started, then the first report operation failed.'
}
finally {
    Pop-Location
}