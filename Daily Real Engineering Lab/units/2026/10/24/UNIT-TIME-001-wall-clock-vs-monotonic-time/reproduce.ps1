$ErrorActionPreference = "Stop"
dotnet run --project "$PSScriptRoot/starter/ClockLab.csproj" -- reproduce
exit $LASTEXITCODE
