$output = dotnet run --project "$PSScriptRoot/starter/RecordEqualityLab.csproj" 2>&1
if ($LASTEXITCODE -ne 0) { exit 1 }
$output | ForEach-Object { Write-Host $_ }
$text = $output -join "`n"
if ($text -match 'first-equals-equivalent:True' -and $text -match 'equivalent-count:1' -and $text -match 'total-count:2') {
  Write-Host 'Verification passed'
  exit 0
}
exit 1
