$ErrorActionPreference = "Stop"
dotnet run --project "$PSScriptRoot/harness/GateScheduleHarness.csproj" -- reproduce
exit $LASTEXITCODE
