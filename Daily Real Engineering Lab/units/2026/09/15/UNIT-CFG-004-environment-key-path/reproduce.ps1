$ErrorActionPreference = 'Stop'
$env:ExportOptions__BatchSize = '25'
$output = dotnet run --project starter/ConfigProbe.csproj -- --probe
Write-Host $output
if ($output -notmatch 'Effective export batch size: 100') {
    throw 'Starter symptom was not reproduced. Expected configured value 25 to be ignored and fallback 100 to appear.'
}
Write-Host 'Reproduced: deployment supplied 25, application reported 100.'
