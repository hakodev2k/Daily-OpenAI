$ErrorActionPreference = 'Stop'

$output = dotnet run --project ./starter/CacheStampedeLab.csproj -- --verify
$output | Write-Host

if ($LASTEXITCODE -ne 0) {
    throw 'Verification FAIL: code trong starter/ chưa đảm bảo một source load cho hot key và giữ đúng functional result.'
}

if (($output -join "`n") -notmatch 'sourceCalls=1') {
    throw 'Verification FAIL: sourceCalls phải bằng 1.'
}

if (($output -join "`n") -notmatch 'allPricesCorrect=True') {
    throw 'Verification FAIL: functional result bị regression.'
}

Write-Host 'Verification PASS.'
