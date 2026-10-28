$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    & dotnet run --project "starter/RequestBodyReplayLab.csproj" -- --run
    if ($LASTEXITCODE -ne 0) { throw "Run failed: exit $LASTEXITCODE" }
} finally { Pop-Location }
