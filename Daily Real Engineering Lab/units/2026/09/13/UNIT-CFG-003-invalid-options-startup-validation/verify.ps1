$ErrorActionPreference = 'Stop'
$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $Root
try {
    $output = (& dotnet run --project ./starter/Starter.csproj --no-launch-profile 2>&1 | Out-String)
    $exitCode = $LASTEXITCODE

    Write-Host $output

    if ($exitCode -eq 0) { throw 'Invalid deployment configuration must be rejected.' }
    if ($output -match 'APP_STARTED') { throw 'Application still reports started before rejecting invalid configuration.' }
    if ($output -match 'REQUEST_FAILED') { throw 'Failure still occurs inside the business operation instead of the startup boundary.' }
    if ($output -notmatch 'Reports:BaseUrl must be an absolute HTTP\(S\) URI') { throw 'Expected an explicit configuration-contract validation message.' }

    Write-Host 'VERIFIED: invalid configuration is rejected before the application reports readiness.'
}
finally {
    Pop-Location
}