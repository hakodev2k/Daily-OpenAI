$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/EnvelopeIncident.csproj'
dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Build failed; this is not a valid reproduction.' }
$output = & dotnet run --no-build --project $project -- --reproduce 2>&1
$code = $LASTEXITCODE
$lines = $output -join [Environment]::NewLine
Write-Host $lines
if ($code -ne 0 -or $lines -notmatch 'ROUNDTRIP_OK=True' -or
    $lines -notmatch 'BYTE_RELATION_EXPOSED=True' -or
    $lines -notmatch 'REPRODUCED:') {
    throw 'Expected starter symptom was not reproduced.'
}
Write-Host 'REPRODUCTION_CONFIRMED'
