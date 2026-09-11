$ErrorActionPreference = 'Stop'

$output = dotnet run --project ./starter/CacheStampedeLab.csproj --
$output | Write-Host

if ($LASTEXITCODE -ne 0) {
    throw 'Không reproduce được symptom mong đợi.'
}

if (($output -join "`n") -notmatch 'sourceCalls=(\d+)') {
    throw 'Không đọc được sourceCalls.'
}

$calls = [int]$Matches[1]
if ($calls -le 1) {
    throw "Expected sourceCalls > 1, actual=$calls"
}

Write-Host "Reproduction PASS: concurrent miss tạo $calls source calls cho một hot key."
