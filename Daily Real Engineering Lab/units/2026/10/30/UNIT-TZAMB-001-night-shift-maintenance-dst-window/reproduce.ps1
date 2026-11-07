$ErrorActionPreference = "Stop"
dotnet run --project (Join-Path $PSScriptRoot "Lab.csproj") -- reproduce
exit $LASTEXITCODE
