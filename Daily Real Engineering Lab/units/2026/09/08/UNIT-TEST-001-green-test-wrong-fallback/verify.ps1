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
    Write-Error "FAIL: probe did not emit FallbackQuote."
    exit 1
}

$quote = [decimal]::Parse($match.Matches.Groups[1].Value, [System.Globalization.CultureInfo]::InvariantCulture)
if ($quote -ne 12.50m) {
    Write-Error "FAIL: expected fallback quote 12.50 but got $quote."
    exit 1
}

Write-Host "PASS: tests are green and timeout behavior satisfies the approved fallback contract."
