$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'setup.ps1')
$scriptPath = Join-Path $PSScriptRoot 'starter/change-detection.ps1'
$cases = @(
    @{ Name = 'case-a'; Expected = @('src/Enrollment/EnrollmentService.cs') },
    @{ Name = 'case-b'; Expected = @('src/Api/Approval.cs') },
    @{ Name = 'case-c'; Expected = @() }
)
foreach ($case in $cases) {
    $repo = Join-Path $PSScriptRoot "workspace/fixtures/$($case.Name)"
    $actual = @(& $scriptPath -RepoPath $repo -BaseRef main -HeadRef feature) |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
    $actualKey = (@($actual | Sort-Object) -join '|')
    $expectedKey = (@($case.Expected | Sort-Object) -join '|')
    if ($actualKey -ne $expectedKey) {
        throw "$($case.Name) FAIL: expected=[$expectedKey]; actual=[$actualKey]"
    }
    Write-Host "$($case.Name) PASS: [$actualKey]"
}
Write-Host 'VERIFIED: ba trường hợp đúng trên starter người học chỉnh sửa.'
