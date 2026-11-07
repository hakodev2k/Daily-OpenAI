$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/BaggageSlaLab.csproj'
$oldMinutes = [Environment]::GetEnvironmentVariable('BAGGAGE_Baggage__AlertAfterMinutes', 'Process')
$oldQueue = [Environment]::GetEnvironmentVariable('BAGGAGE_Baggage__EscalationQueue', 'Process')
try {
    dotnet build $project --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Build failed; this is not a valid reproduction.' }
    [Environment]::SetEnvironmentVariable('BAGGAGE_Baggage__AlertAfterMinutes', '5', 'Process')
    [Environment]::SetEnvironmentVariable('BAGGAGE_Baggage__EscalationQueue', 'priority', 'Process')
    $output = & dotnet run --no-build --project $project -- --assert-contract 2>&1
    $code = $LASTEXITCODE
    $lines = $output -join "`n"
    Write-Host $lines
    if ($code -ne 2 -or $lines -notmatch 'EFFECTIVE_ALERT_MINUTES=30' -or $lines -notmatch 'EXPECTED_ALERT_MINUTES=5' -or $lines -notmatch 'CONTRACT_FAIL') {
        throw 'Expected starter symptom was not reproduced. Inspect build, environment, and output.'
    }
    Write-Host 'REPRODUCED: deployed override differs from effective settings.'
}
finally {
    [Environment]::SetEnvironmentVariable('BAGGAGE_Baggage__AlertAfterMinutes', $oldMinutes, 'Process')
    [Environment]::SetEnvironmentVariable('BAGGAGE_Baggage__EscalationQueue', $oldQueue, 'Process')
}
