$ErrorActionPreference = 'Stop'
$output = dotnet run --project "$PSScriptRoot/starter/RetryAfterLab.csproj"
$output | ForEach-Object { Write-Host $_ }

$delta = [double](($output | Where-Object { $_ -like 'DELTA_DELAY=*' }) -replace 'DELTA_DELAY=','')
$date = [double](($output | Where-Object { $_ -like 'DATE_DELAY=*' }) -replace 'DATE_DELAY=','')

if ($delta -ge 4 -and $delta -le 6 -and $date -eq 0) {
    Write-Host 'REPRODUCED: one valid Retry-After representation falls back to immediate retry.'
    exit 0
}

Write-Error 'Expected starter symptom was not reproduced.'