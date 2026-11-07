$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/FieldDispatch.csproj" -- verify
if ($LASTEXITCODE -ne 0) { throw "Learner starter failed verification (exit $LASTEXITCODE)." }
Write-Host "VERIFIED"
