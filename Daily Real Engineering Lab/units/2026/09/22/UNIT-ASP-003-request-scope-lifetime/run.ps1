$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/Starter.csproj" -- --mode run
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }