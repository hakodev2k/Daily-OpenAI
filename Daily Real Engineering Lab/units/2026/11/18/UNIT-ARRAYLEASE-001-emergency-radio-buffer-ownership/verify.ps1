$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/RadioDispatch.csproj" -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
