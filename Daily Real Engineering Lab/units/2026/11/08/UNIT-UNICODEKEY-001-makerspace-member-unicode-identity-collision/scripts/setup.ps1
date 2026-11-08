$ErrorActionPreference = "Stop"
$p = Join-Path $PSScriptRoot "../starter/IdentityLab.csproj"
dotnet restore $p
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build $p --no-restore
exit $LASTEXITCODE
