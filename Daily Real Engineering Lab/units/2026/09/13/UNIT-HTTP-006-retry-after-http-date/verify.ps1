$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/RetryAfterLab.csproj"
$output | ForEach-Object { Write-Host $_ }

$delta = [double](($output | Where-Object { $_ -like 'DELTA_DELAY=*' }) -replace 'DELTA_DELAY=','')
$date = [double](($output | Where-Object { $_ -like 'DATE_DELAY=*' }) -replace 'DATE_DELAY=','')

if ($delta -ge 4 -and $delta -le 6 -and $date -ge 4 -and $date -le 6) {
    Write-Host 'VERIFIED: both standard Retry-After representations produce a bounded delay.'
    exit 0
}

Write-Error 'Verification failed. Both valid representations must produce an equivalent retry delay.'