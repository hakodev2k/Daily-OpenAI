$ErrorActionPreference = 'Stop'

& (Join-Path $PSScriptRoot 'starter/setup-repo.ps1') | Out-Null
$repo = Join-Path $PSScriptRoot 'workspace/repo'

Push-Location $repo
try {
    git ls-files --error-unmatch appsettings.Development.json *> $null
    $tracked = ($LASTEXITCODE -eq 0)

    $status = (git status --short -- appsettings.Development.json) -join "`n"

    git check-ignore -q appsettings.Development.json
    $ignored = ($LASTEXITCODE -eq 0)

    Write-Host "tracked=$([int]$tracked)"
    Write-Host "ignored=$([int]$ignored)"
    Write-Host "status=$status"

    if ($tracked -and -not [string]::IsNullOrWhiteSpace($status)) {
        Write-Host 'PASS: symptom reproduced — local settings are still reported as a tracked modification.'
        exit 0
    }

    Write-Host 'FAIL: expected tracked-file symptom was not reproduced.'
    exit 1
}
finally {
    Pop-Location
}
