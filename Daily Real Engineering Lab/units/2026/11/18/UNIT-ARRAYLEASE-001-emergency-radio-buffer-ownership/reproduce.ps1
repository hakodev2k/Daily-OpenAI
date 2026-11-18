$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/RadioDispatch.csproj" -- reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
