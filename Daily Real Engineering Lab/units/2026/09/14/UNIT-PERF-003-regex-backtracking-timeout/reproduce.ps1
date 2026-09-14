$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'starter/RegexPerformanceLab.csproj'
$output = dotnet run --project $project --configuration Release 2>&1 | Out-String
$output | Write-Host

if ($LASTEXITCODE -ne 0) {
    throw "Starter failed to run."
}

$match = [regex]::Match($output, 'ADVERSARIAL_TIMEOUTS=(\d+)')
if (-not $match.Success) {
    throw "Could not find ADVERSARIAL_TIMEOUTS in starter output."
}

$timeouts = [int]$match.Groups[1].Value
if ($timeouts -lt 1) {
    throw "Expected at least one timeout. If your machine is unusually fast, increase MaxLength gradually in starter/Program.cs."
}

Write-Host "Reproduction confirmed: at least one near-match input hit the validator timeout."
