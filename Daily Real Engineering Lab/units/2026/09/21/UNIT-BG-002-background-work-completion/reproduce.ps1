$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Expected starter symptom was not reproduced.' }
Write-Host 'REPRODUCED: batch completes before required item work reaches a terminal state.'