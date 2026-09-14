$output = dotnet run --project "$PSScriptRoot/starter/ClosureLab.csproj" 2>&1
if ($LASTEXITCODE -ne 0) { exit 1 }
$lines = @($output | Where-Object { $_ -like 'processed:*' })
$expected = @('processed:north','processed:central','processed:south')
if ($lines.Count -ne 3) { exit 1 }
foreach ($item in $expected) {
  if (($lines | Where-Object { $_ -eq $item }).Count -ne 1) { exit 1 }
}
Write-Host 'Verification passed'
