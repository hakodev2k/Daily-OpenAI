$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/BaggageSlaLab.csproj'
$oldMinutes = [Environment]::GetEnvironmentVariable('BAGGAGE_Baggage__AlertAfterMinutes', 'Process')
$oldQueue = [Environment]::GetEnvironmentVariable('BAGGAGE_Baggage__EscalationQueue', 'Process')
try {
    dotnet build $project --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Learner build failed.' }

    foreach ($minutes in @('5', '11')) {
        [Environment]::SetEnvironmentVariable('BAGGAGE_Baggage__AlertAfterMinutes', $minutes, 'Process')
        [Environment]::SetEnvironmentVariable('BAGGAGE_Baggage__EscalationQueue', 'priority', 'Process')
        $output = & dotnet run --no-build --project $project -- --assert-contract 2>&1
        $code = $LASTEXITCODE
        $lines = $output -join "`n"
        Write-Host $lines
        if ($code -ne 0 -or $lines -notmatch 'CONTRACT_PASS' -or $lines -notmatch ('EFFECTIVE_ALERT_MINUTES=' + $minutes) -or $lines -notmatch 'EFFECTIVE_ESCALATION_QUEUE=priority') {
            throw ('Override contract failed for threshold ' + $minutes)
        }
    }

    [Environment]::SetEnvironmentVariable('BAGGAGE_Baggage__AlertAfterMinutes', $null, 'Process')
    [Environment]::SetEnvironmentVariable('BAGGAGE_Baggage__EscalationQueue', $null, 'Process')
    $output = & dotnet run --no-build --project $project -- --assert-contract 2>&1
    $code = $LASTEXITCODE
    $lines = $output -join "`n"
    Write-Host $lines
    if ($code -ne 0 -or $lines -notmatch 'CONTRACT_PASS' -or $lines -notmatch 'EFFECTIVE_ALERT_MINUTES=30' -or $lines -notmatch 'EFFECTIVE_ESCALATION_QUEUE=standard') {
        throw 'Default fallback contract failed.'
    }
    Write-Host 'VERIFIED: deployment overrides and defaults behave correctly.'
}
finally {
    [Environment]::SetEnvironmentVariable('BAGGAGE_Baggage__AlertAfterMinutes', $oldMinutes, 'Process')
    [Environment]::SetEnvironmentVariable('BAGGAGE_Baggage__EscalationQueue', $oldQueue, 'Process')
}
