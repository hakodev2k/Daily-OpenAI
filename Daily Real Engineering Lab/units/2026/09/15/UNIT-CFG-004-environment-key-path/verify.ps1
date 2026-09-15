$ErrorActionPreference = 'Stop'
$env:ExportOptions__BatchSize = '25'
$output = dotnet run --project starter/ConfigProbe.csproj -- --probe
Write-Host $output
if ($output -notmatch 'Effective export batch size: 25') {
    throw 'Verification failed: learner-editable starter still does not consume the deployment value 25.'
}
Write-Host 'Verified: starter consumes the environment-specific batch size.'
