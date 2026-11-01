$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Push-Location $root
try {
    dotnet build './starter/ProcessPipeLab.csproj' -c Release
    if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
    $dll = './starter/bin/Release/net8.0/ProcessPipeLab.dll'
    $smallOutput = & dotnet $dll '--bytes=128' 2>&1
    $smallExit = $LASTEXITCODE
    if ($smallExit -ne 0 -or (($smallOutput | Out-String) -notmatch 'STREAMS_DRAINED: bytes=128')) {
        throw "Small control case failed: exit=$smallExit output=$smallOutput"
    }
    $largeOutput = & dotnet $dll '--bytes=524288' 2>&1
    $largeExit = $LASTEXITCODE
    if ($largeExit -ne 23 -or (($largeOutput | Out-String) -notmatch 'DEADLOCK_REPRODUCED')) {
        throw "Expected starter symptom missing: exit=$largeExit output=$largeOutput"
    }
    Write-Host 'REPRODUCE_PASS: small case succeeded; large case timed out as expected.'
}
finally { Pop-Location }
