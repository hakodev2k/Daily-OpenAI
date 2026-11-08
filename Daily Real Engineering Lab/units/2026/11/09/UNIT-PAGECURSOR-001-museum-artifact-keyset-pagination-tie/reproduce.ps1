$ErrorActionPreference='Stop'
dotnet run --project "$PSScriptRoot/starter/ExportLab.csproj" -- --reproduce
exit $LASTEXITCODE
