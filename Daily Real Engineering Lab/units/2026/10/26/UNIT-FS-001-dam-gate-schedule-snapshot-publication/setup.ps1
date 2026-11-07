$ErrorActionPreference = "Stop"
dotnet build "$PSScriptRoot/harness/GateScheduleHarness.csproj" --nologo
exit $LASTEXITCODE
