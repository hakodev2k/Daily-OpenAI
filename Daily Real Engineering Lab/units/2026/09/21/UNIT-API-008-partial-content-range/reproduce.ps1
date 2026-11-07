$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Expected starter symptom was not reproduced.' }
Write-Host 'REPRODUCED: inspect Content-Range and actual body length.'