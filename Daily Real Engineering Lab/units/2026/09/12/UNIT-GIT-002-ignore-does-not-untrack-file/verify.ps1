$ErrorActionPreference = 'Stop'

$repo = Join-Path $PSScriptRoot 'workspace/repo'
if (-not (Test-Path (Join-Path $repo '.git'))) {
    Write-Host 'FAIL: run ./reproduce.ps1 first.'
    exit 1
}

Push-Location $repo
try {
    if (-not (Test-Path 'appsettings.Development.json')) {
        Write-Host 'FAIL: local settings file must remain on disk.'
        exit 1
    }

    git check-ignore -q appsettings.Development.json
    if ($LASTEXITCODE -ne 0) {
        Write-Host 'FAIL: file is not covered by ignore rules.'
        exit 1
    }

    git ls-files --error-unmatch appsettings.Development.json *> $null
    if ($LASTEXITCODE -eq 0) {
        Write-Host 'FAIL: file is still tracked in the index.'
        exit 1
    }

    $content = Get-Content 'appsettings.Development.json' -Raw
    if ($content -notmatch 'local\.partner\.test') {
        Write-Host 'FAIL: local working copy was not preserved.'
        exit 1
    }

    Write-Host 'PASS: local file is preserved, ignored, and no longer tracked.'
    exit 0
}
finally {
    Pop-Location
}
