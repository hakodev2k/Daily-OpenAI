$ErrorActionPreference = "Stop"
dotnet run --project (Join-Path $PSScriptRoot "Lab.csproj") -- verify
exit $LASTEXITCODE
