$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/RedisOutageLab.csproj" -- --reproduce
exit $LASTEXITCODE
