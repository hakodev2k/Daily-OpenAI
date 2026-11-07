$ErrorActionPreference = "Stop"
$work = Join-Path $PSScriptRoot ".verify-work"
$source = Join-Path $work "source"
$clone = Join-Path $work "ci-clone"

Remove-Item $work -Recurse -Force -ErrorAction SilentlyContinue
New-Item $source -ItemType Directory -Force | Out-Null

Push-Location $source
try {
    git init -q
    git config user.email "lab@example.local"
    git config user.name "Engineering Lab"
    "release" | Set-Content app.txt
    git add app.txt
    git commit -q -m "release"
    git tag v1.2.0
    "next" | Add-Content app.txt
    git add app.txt
    git commit -q -m "post release"
}
finally { Pop-Location }

$sourceUri = ([System.Uri]$source).AbsoluteUri
git clone -q --depth 1 $sourceUri $clone
if ($LASTEXITCODE -ne 0) { throw "Unable to create shallow CI clone." }

$version = & "$PSScriptRoot/starter/Get-Version.ps1" -RepositoryPath $clone
Write-Host "verifiedVersion=$version"

if ($version -ne "1.2.0") {
    throw "Expected 1.2.0 from learner code, got '$version'."
}

Write-Host "LAB_VERIFY_PASS"
