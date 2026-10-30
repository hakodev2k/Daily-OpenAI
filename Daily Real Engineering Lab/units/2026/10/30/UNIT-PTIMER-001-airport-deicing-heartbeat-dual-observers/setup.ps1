$ErrorActionPreference = "Stop"
dotnet restore (Join-Path $PSScriptRoot "Lab.csproj")
exit $LASTEXITCODE
