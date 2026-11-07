$ErrorActionPreference = 'Stop'
dotnet restore "$PSScriptRoot/starter/Starter.csproj"
dotnet run --project "$PSScriptRoot/starter/Starter.csproj" -- verify
if ($LASTEXITCODE -ne 0) { throw "Verification failed with exit code $LASTEXITCODE" }
