$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/NurseryCsv.csproj" -- demo
exit $LASTEXITCODE
