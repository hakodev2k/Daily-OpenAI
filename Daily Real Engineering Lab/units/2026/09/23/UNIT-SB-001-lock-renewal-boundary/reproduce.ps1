$ErrorActionPreference='Stop'
dotnet run --project ./starter/Starter.csproj
if ($LASTEXITCODE -eq 0) { throw 'Expected starter incident was not reproduced.' }
Write-Host 'Reproduced: one logical job produced duplicate completion side effects.'