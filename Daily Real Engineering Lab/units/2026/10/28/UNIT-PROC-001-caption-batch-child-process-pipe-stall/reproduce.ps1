$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/CaptionPipelineLab.csproj'
dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Build failed; reproduction invalid.' }
$output = & dotnet run --no-build --project $project -- --reproduce 2>&1
$code = $LASTEXITCODE
$lines = $output -join [Environment]::NewLine
Write-Host $lines
if ($code -ne 0 -or $lines -notmatch 'CHILD_EXIT_NOT_OBSERVED_BEFORE_DEADLINE=True' -or $lines -notmatch 'REPRODUCED:') {
    throw 'Original starter symptom not reproduced.'
}
Write-Host 'REPRODUCTION_CONFIRMED'
