$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Push-Location $root
try {
    dotnet build './starter/ProcessPipeLab.csproj' -c Release
    if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
    $dll = './starter/bin/Release/net8.0/ProcessPipeLab.dll'
    function Assert-Run([int]$bytes, [int]$childExit, [int]$expectedExit, [string]$marker) {
        $output = & dotnet $dll "--bytes=$bytes" "--child-exit=$childExit" 2>&1
        $actualExit = $LASTEXITCODE
        $capturedText = $output | Out-String
        if ($actualExit -ne $expectedExit -or $capturedText -notmatch [regex]::Escape($marker)) {
            throw "VERIFY_FAIL: bytes=$bytes childExit=$childExit actual=$actualExit expected=$expectedExit output=$capturedText"
        }
        Write-Host "CASE_PASS: bytes=$bytes childExit=$childExit actualExit=$actualExit"
    }
    Assert-Run 128 0 0 'STREAMS_DRAINED: bytes=128'
    Assert-Run 131072 0 0 'STREAMS_DRAINED: bytes=131072'
    Assert-Run 524288 0 0 'STREAMS_DRAINED: bytes=524288'
    Assert-Run 524288 7 31 'CHILD_FAILED: exit=7'
    Write-Host 'VERIFY_PASS: learner-edited starter passed all behavioral checks.'
}
finally { Pop-Location }
