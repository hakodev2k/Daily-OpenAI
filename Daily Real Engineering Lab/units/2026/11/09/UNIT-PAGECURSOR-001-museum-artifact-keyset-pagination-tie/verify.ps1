$ErrorActionPreference='Stop'
dotnet run --project "$PSScriptRoot/starter/ExportLab.csproj" -- --verify
exit $LASTEXITCODE
