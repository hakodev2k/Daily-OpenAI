$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- --sequential
if ($LASTEXITCODE -ne 0) { throw 'Sequential baseline failed.' }
dotnet run --project ./starter/Starter.csproj