$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PortManifestLab.csproj'
dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Build failed; reproduction invalid.' }
$output = & dotnet run --no-build --project $project -- --reproduce 2>&1
$code = $LASTEXITCODE
$lines = $output -join [Environment]::NewLine
Write-Host $lines
if ($code -ne 0 -or $lines -notmatch 'AMBIGUOUS_ACCEPTED=True' -or
    $lines -notmatch 'INSPECTION_DOWNGRADED=True' -or
    $lines -notmatch 'REPRODUCED:') {
    throw 'Starter symptom not reproduced. Check original starter source.'
}
Write-Host 'REPRODUCTION_CONFIRMED'
