param(
    [string]$RepositoryPath = "."
)

$ErrorActionPreference = "Stop"

Push-Location $RepositoryPath
try {
    $isShallow = (git rev-parse --is-shallow-repository).Trim()
    if ($LASTEXITCODE -ne 0) { throw "Not a Git repository." }

    if ($isShallow -eq "true") {
        git fetch --tags --unshallow -q
        if ($LASTEXITCODE -ne 0) { throw "Unable to recover Git history required for version derivation." }
    }

    $tag = git describe --tags --abbrev=0 2>$null
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($tag)) {
        throw "No release tag is available; refusing to publish a fallback release version."
    }

    $version = $tag.Trim()
    if ($version.StartsWith("v")) { $version = $version.Substring(1) }
    Write-Output $version
}
finally {
    Pop-Location
}
