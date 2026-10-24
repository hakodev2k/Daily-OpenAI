$ErrorActionPreference = "Stop"
dotnet restore "$PSScriptRoot/starter/ClockLab.csproj"
exit $LASTEXITCODE
