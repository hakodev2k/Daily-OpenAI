$project = Join-Path $PSScriptRoot '../baseline/JsonLab.csproj'
$output = & dotnet run --project $project --configuration Release 2>&1 | Out-String
$exitCode = $LASTEXITCODE
Write-Output $output
if ($exitCode -eq 0 -or $output -notmatch 'CHECK_FAILED.*ObjectDisposedException') {
    throw 'Reproduction failed: expected runtime failure was not observed.'
}
Write-Output 'REPRODUCTION_CONFIRMED'
