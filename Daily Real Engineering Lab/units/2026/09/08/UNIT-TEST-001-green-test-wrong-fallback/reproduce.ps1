$ErrorActionPreference = "Stop"
$root = Join-Path $PSScriptRoot "starter"
$tests = Join-Path $root "Shipping.Tests/Shipping.Tests.csproj"
$probe = Join-Path $root "Shipping.Probe/Shipping.Probe.csproj"

dotnet restore $tests | Out-Null
dotnet test $tests --no-restore

$output = dotnet run --project $probe
Write-Host $output

$match = $output | Select-String '^FallbackQuote=(.+)$'
if (-not $match) {
    Write-Error "REPRODUCE FAILED: probe did not emit FallbackQuote."
    exit 1
}

$quote = [decimal]::Parse($match.Matches.Groups[1].Value, [System.Globalization.CultureInfo]::InvariantCulture)
if ($quote -eq 12.50m) {
    Write-Error "REPRODUCE FAILED: starter already satisfies the fallback contract."
    exit 1
}

Write-Host "REPRODUCED: tests are green while the fallback quote violates the 12.50 contract."
