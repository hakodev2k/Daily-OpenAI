$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$repo = Join-Path $root 'workspace/repo'

if (Test-Path $repo) {
    Remove-Item $repo -Recurse -Force
}

New-Item -ItemType Directory -Path $repo -Force | Out-Null
Push-Location $repo
try {
    git init -q
    if ($LASTEXITCODE -ne 0) { throw 'git init failed' }

    git config user.email 'lab@example.test'
    git config user.name 'Engineering Lab'

    @'
{
  "PartnerEndpoint": "https://sandbox.partner.test"
}
'@ | Set-Content -Path 'appsettings.Development.json'

    git add appsettings.Development.json
    git commit -q -m 'Add development settings'
    if ($LASTEXITCODE -ne 0) { throw 'initial commit failed' }

    'appsettings.Development.json' | Set-Content -Path '.gitignore'
    git add .gitignore
    git commit -q -m 'Ignore local development settings'
    if ($LASTEXITCODE -ne 0) { throw 'ignore-rule commit failed' }

    @'
{
  "PartnerEndpoint": "https://local.partner.test"
}
'@ | Set-Content -Path 'appsettings.Development.json'
}
finally {
    Pop-Location
}

Write-Host "Workspace prepared at $repo"
