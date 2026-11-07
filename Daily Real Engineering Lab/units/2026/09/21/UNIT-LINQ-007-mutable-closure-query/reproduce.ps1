$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'REPRODUCE_FAIL: intended starter symptom was not observed.' }
Write-Host 'REPRODUCED: export differs from the report-boundary expectation.'