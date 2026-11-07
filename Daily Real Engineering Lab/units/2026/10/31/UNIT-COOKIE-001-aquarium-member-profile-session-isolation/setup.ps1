$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "harness/PartnerCookieHarness.csproj"
dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build $project -c Release --no-restore
exit $LASTEXITCODE
