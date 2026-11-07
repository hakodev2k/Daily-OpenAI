$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root 'starter/Lab.csproj'
$out = Join-Path $root '.artifacts/repro'

Remove-Item $out -Recurse -Force -ErrorAction SilentlyContinue

$local = dotnet run --project $project 2>&1 | Out-String
if ($LASTEXITCODE -ne 0 -or $local -notmatch 'FORMATTED:ACME') {
    throw "Starter local run did not produce expected baseline. Output:`n$local"
}

& dotnet publish $project -c Release -r win-x64 --self-contained true -p:PublishTrimmed=true -o $out | Out-Host
if ($LASTEXITCODE -ne 0) { throw 'Publish failed before reproduction could run.' }

$exe = Join-Path $out 'Lab.exe'
$published = & $exe 2>&1 | Out-String
$publishedExit = $LASTEXITCODE

Write-Host "Local run: $($local.Trim())"
Write-Host "Trimmed run: $($published.Trim())"

if ($publishedExit -eq 0) {
    throw 'Reproduction failed: trimmed artifact unexpectedly completed successfully. Reset starter state and retry.'
}
if ($published -notmatch 'PLUGIN_NOT_FOUND') {
    throw "Reproduction failed with an unexpected symptom. Output:`n$published"
}

Write-Host 'REPRODUCED: local run succeeds while trimmed artifact cannot create configured formatter.'