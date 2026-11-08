$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/DonationLedger.csproj'
$cases = @(
    @{ input = @('0.005','0.005','0.005'); expected = '0.03' },
    @{ input = @('0.005','0.015'); expected = '0.03' },
    @{ input = @('0.015','0.005'); expected = '0.03' },
    @{ input = @('1.004','2.006'); expected = '3.01' },
    @{ input = @('0.10','0.20'); expected = '0.30' },
    @{ input = @('-0.005','0.015'); expected = '0.01' }
)
dotnet build $project --nologo | Out-Host
if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
foreach ($case in $cases) {
    $actual = (& dotnet run --no-build --project $project --no-launch-profile -- @($case.input) | Select-Object -Last 1).Trim()
    if ($LASTEXITCODE -ne 0) { throw 'Execution failed' }
    if ($actual -ne $case.expected) { throw "FAIL: $($case.input -join ',') expected=$($case.expected) actual=$actual" }
}
Write-Host 'PASS: all learner-workspace contract cases.'
