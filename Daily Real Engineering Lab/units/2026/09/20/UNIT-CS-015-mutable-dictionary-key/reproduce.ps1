$output = dotnet run --project ./starter/MutableKeyLab.csproj | Out-String
$output
if ($output -notmatch 'lookup-before=True' -or $output -notmatch 'lookup-after=False') { throw 'Expected starter symptom was not reproduced.' }
Write-Host 'REPRODUCE_PASS'