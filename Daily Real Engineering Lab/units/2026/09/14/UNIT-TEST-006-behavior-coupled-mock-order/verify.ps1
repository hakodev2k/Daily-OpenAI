$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'starter/Notification.Tests.csproj'

dotnet restore $project
dotnet test $project --no-restore --nologo

if ($LASTEXITCODE -ne 0) {
    throw 'Verification failed. The learner-editable starter test suite is still red.'
}

Write-Host 'Verification passed: the learner-editable test suite is green and still checks the notification contract.'
