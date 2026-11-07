$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'starter/Notification.Tests.csproj'

dotnet restore $project
$output = dotnet test $project --no-restore --nologo 2>&1
$exitCode = $LASTEXITCODE
$output | Write-Host

if ($exitCode -eq 0) {
    throw 'Expected the starter test suite to fail, but it passed. The lab may already be modified.'
}

$text = $output -join "`n"
if ($text -notmatch 'Deliver_preserves_notification_contract') {
    throw 'The test suite failed, but not at the expected lab test.'
}

Write-Host 'Reproduction succeeded: business assertions execute, but the starter test suite reports the intended regression signal.'
exit 0
