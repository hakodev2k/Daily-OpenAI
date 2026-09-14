$output = dotnet run --project "$PSScriptRoot/starter/RecordEqualityLab.csproj" 2>&1
if ($LASTEXITCODE -ne 0) { exit 1 }
$output | ForEach-Object { Write-Host $_ }
$text = $output -join "`n"
if ($text -match 'equivalent-count:2' -and $text -match 'total-count:3') {
  Write-Host 'Reproduction confirmed'
  exit 0
}
exit 1
