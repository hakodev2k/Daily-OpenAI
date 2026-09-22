$ErrorActionPreference = 'Stop'
$base = 'http://127.0.0.1:5092'

function Get-Location([string]$returnUrl) {
    try {
        Invoke-WebRequest -Method Post -Uri "$base/login?returnUrl=$([uri]::EscapeDataString($returnUrl))" -MaximumRedirection 0 -ErrorAction Stop | Out-Null
    } catch {
        $response = $_.Exception.Response
        if ($null -eq $response) { throw }
        return $response.Headers.Location.ToString()
    }
}

$cases = @(
    @{ Name='valid local path'; Input='/profile'; Expected='/profile' },
    @{ Name='external absolute URL'; Input='https://example.com/after-login'; Expected='/' },
    @{ Name='scheme-relative external URL'; Input='//example.com/after-login'; Expected='/' }
)

foreach ($case in $cases) {
    $actual = Get-Location $case.Input
    Write-Host "$($case.Name): $actual"
    if ($actual -ne $case.Expected) {
        throw "Verification failed for '$($case.Name)'. Expected '$($case.Expected)', got '$actual'."
    }
}

Write-Host 'Verification passed: valid local navigation is preserved and untrusted destinations use the safe fallback.'