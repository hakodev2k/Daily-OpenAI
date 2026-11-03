$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/FieldDispatch.csproj" -- reproduce
if ($LASTEXITCODE -ne 0) { throw "Reproduction did not detect the expected fault (exit $LASTEXITCODE)." }
Write-Host "REPRODUCED"
