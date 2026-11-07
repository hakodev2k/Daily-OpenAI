$output = dotnet run --project "$PSScriptRoot/starter/Lab.csproj" 2>&1
$output | ForEach-Object { Write-Host $_ }
$text = $output -join "`n"
if ($text -match "PaymentRepository.ThrowFromRepository") { Write-Error "Expected source frame to be missing before the fix."; exit 1 }
Write-Host "REPRODUCED: original source frame is missing from the stack trace."
