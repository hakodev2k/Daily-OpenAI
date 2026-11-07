$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root 'starter/Lab.csproj'
$out = Join-Path $root '.artifacts/verify'

Remove-Item $out -Recurse -Force -ErrorAction SilentlyContinue
& dotnet publish $project -c Release -r win-x64 --self-contained true -p:PublishTrimmed=true -o $out | Out-Host
if ($LASTEXITCODE -ne 0) { throw 'Trimmed publish failed.' }

$exe = Join-Path $out 'Lab.exe'
$result = & $exe 2>&1 | Out-String
$exit = $LASTEXITCODE

if ($exit -ne 0) {
    throw "Verification failed: trimmed artifact exited with code $exit. Output:`n$result"
}
if ($result.Trim() -ne 'FORMATTED:ACME') {
    throw "Verification failed: expected FORMATTED:ACME. Output:`n$result"
}

Write-Host 'VERIFIED: learner-edited starter works as a trimmed self-contained artifact.'