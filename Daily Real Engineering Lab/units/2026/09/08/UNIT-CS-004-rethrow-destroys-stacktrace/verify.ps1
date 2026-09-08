$output = dotnet run --project "$PSScriptRoot/starter/Lab.csproj" 2>&1
$output | ForEach-Object { Write-Host $_ }
$text = $output -join "`n"
if ($text -notmatch "PaymentRepository.ThrowFromRepository") { Write-Error "FAIL: original exception source frame is still missing."; exit 1 }
Write-Host "PASS: original stack trace is preserved."
