$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/RegexIncident.csproj'
$output = dotnet run --project $project -- --reproduce 2>&1
$output | ForEach-Object { Write-Host $_ }
if (($LASTEXITCODE -ne 0) -or (-not ($output -match 'REGEX_TIMEOUT=1'))) {
    throw 'Expected starter symptom was not reproduced. See README troubleshooting guidance.'
}
