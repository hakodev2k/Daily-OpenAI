$ErrorActionPreference = 'Stop'
$output = dotnet test "$PSScriptRoot/starter/Invoice.Tests.csproj" --nologo 2>&1
$output | ForEach-Object { Write-Host $_ }
if ($LASTEXITCODE -ne 0) { throw 'Starter test suite did not pass.' }
$source = Get-Content "$PSScriptRoot/starter/InvoiceDispatcherTests.cs" -Raw
if ($source -match 'await\s+Assert\.ThrowsAsync' -or $source -match 'return\s+Assert\.ThrowsAsync') {
  throw 'Starter no longer contains the intended unobserved async assertion symptom.'
}
Write-Host 'REPRODUCED: test suite passes while async assertion completion is not observed by the test method.'
