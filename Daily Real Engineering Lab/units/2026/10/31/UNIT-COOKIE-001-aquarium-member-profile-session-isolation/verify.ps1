$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "harness/PartnerCookieHarness.csproj"
dotnet run --project $project -c Release -- verify
exit $LASTEXITCODE
