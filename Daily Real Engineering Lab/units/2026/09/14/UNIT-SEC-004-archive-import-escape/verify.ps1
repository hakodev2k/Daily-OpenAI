$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false

$project = Join-Path $PSScriptRoot 'starter/ArchiveImportLab.csproj'
$output = & dotnet run --project $project 2>&1
$exitCode = $LASTEXITCODE
$output | ForEach-Object { Write-Host $_ }

if ($exitCode -ne 0) {
    throw "Verification failed. Expected exit code 0 after your fix, but got $exitCode."
}

$text = $output -join "`n"
if ($text -notmatch 'IMPORT_CONTAINED') {
    throw 'Verification failed: containment success marker was not produced.'
}

if ($text -notmatch 'Valid content present: True') {
    throw 'Verification failed: valid archive content no longer imports correctly.'
}

if ($text -notmatch 'File outside import root present: False') {
    throw 'Verification failed: a file was still created outside the import root.'
}

Write-Host 'Verification passed: valid content is preserved and archive writes remain inside the approved import root.'
