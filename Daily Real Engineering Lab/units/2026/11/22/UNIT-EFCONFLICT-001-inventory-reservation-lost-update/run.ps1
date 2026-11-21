$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/SeatLab.csproj"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
