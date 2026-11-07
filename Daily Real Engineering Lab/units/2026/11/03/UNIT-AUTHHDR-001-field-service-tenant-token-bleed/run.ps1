$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/FieldDispatch.csproj" -- reproduce
if ($LASTEXITCODE -ne 0) { throw "Run failed (exit $LASTEXITCODE)." }
