$ErrorActionPreference = "Stop"
dotnet run --project "$PSScriptRoot/starter/ClockLab.csproj" -- verify
exit $LASTEXITCODE
