$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'setup.ps1')
foreach ($caseName in @('case-a','case-b','case-c')) {
    $repo = Join-Path $PSScriptRoot "workspace/fixtures/$caseName"
    $actual = @(& (Join-Path $PSScriptRoot 'starter/change-detection.ps1') -RepoPath $repo -BaseRef main -HeadRef feature)
    Write-Host "$caseName -> $($actual -join ', ')"
}
