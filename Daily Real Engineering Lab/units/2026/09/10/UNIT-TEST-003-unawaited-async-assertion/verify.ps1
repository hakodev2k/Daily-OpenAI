$ErrorActionPreference = 'Stop'
$project = "$PSScriptRoot/starter/Invoice.Tests.csproj"
$sourcePath = "$PSScriptRoot/starter/InvoiceDispatcherTests.cs"
$source = Get-Content $sourcePath -Raw
if ($source -notmatch 'await\s+Assert\.ThrowsAsync' -and $source -notmatch 'return\s+Assert\.ThrowsAsync') {
  throw 'Verification failed: the async assertion is still not observed by the test method.'
}
dotnet test $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Verification failed: learner test suite is not green.' }
Write-Host 'VERIFIED: async assertion completion is observed by the test method and the expected exception contract passes.'
