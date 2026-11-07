$ErrorActionPreference = "Stop"
dotnet run --project "$PSScriptRoot/harness/GateScheduleHarness.csproj" -- observe
exit $LASTEXITCODE
