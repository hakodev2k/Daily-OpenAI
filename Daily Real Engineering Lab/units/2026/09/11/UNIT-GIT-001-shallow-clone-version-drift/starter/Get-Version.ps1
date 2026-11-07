param(
    [string]$RepositoryPath = "."
)

$ErrorActionPreference = "Stop"

Push-Location $RepositoryPath
try {
    $tag = git describe --tags --abbrev=0 2>$null
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($tag)) {
        Write-Output "0.0.0-dev"
        exit 0
    }

    $version = $tag.Trim()
    if ($version.StartsWith("v")) {
        $version = $version.Substring(1)
    }

    Write-Output $version
}
finally {
    Pop-Location
}
