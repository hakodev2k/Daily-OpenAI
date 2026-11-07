$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false

$project = Join-Path $PSScriptRoot 'starter/ArchiveImportLab.csproj'
$output = & dotnet run --project $project 2>&1
$exitCode = $LASTEXITCODE
$output | ForEach-Object { Write-Host $_ }

if ($exitCode -ne 2) {
    throw "Expected starter to report the boundary violation with exit code 2, but got $exitCode."
}

if (($output -join "`n") -notmatch 'BOUNDARY_VIOLATION_DETECTED') {
    throw 'Expected boundary-violation evidence was not produced.'
}

Write-Host 'Reproduction succeeded: the starter created valid content and also wrote outside the approved import root.'
