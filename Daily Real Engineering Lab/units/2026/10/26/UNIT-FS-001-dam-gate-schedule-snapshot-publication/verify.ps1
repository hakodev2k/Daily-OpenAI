$ErrorActionPreference = "Stop"
dotnet run --project "$PSScriptRoot/harness/GateScheduleHarness.csproj" -- verify
exit $LASTEXITCODE
