$ErrorActionPreference = "Stop"
dotnet run --project "$PSScriptRoot/starter/ClockLab.csproj" -- demo
exit $LASTEXITCODE
