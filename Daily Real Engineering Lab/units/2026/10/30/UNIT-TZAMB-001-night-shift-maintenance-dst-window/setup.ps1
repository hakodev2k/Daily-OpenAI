$ErrorActionPreference = "Stop"
dotnet restore (Join-Path $PSScriptRoot "Lab.csproj")
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build (Join-Path $PSScriptRoot "Lab.csproj") --no-restore
exit $LASTEXITCODE
