$ErrorActionPreference = "Stop"
dotnet run --project "$PSScriptRoot/starter/TimeoutOrphanWorkLab.csproj"
exit $LASTEXITCODE
