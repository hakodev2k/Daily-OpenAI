$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'

Write-Host 'Building starter...'
dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host 'Running reproduction...'
$output = & dotnet run --project $project --no-build --nologo 2>&1 | Out-String
$exitCode = $LASTEXITCODE
Write-Host $output

if ($exitCode -eq 1 -and $output -match 'SYMPTOM: request 3 did not receive response headers') {
    Write-Host 'Reproduction confirmed.'
    exit 0
}

Write-Error "Expected the controlled starter symptom, but it was not observed. Process exit code: $exitCode"
exit 1
