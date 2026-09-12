$ErrorActionPreference = 'Stop'
dotnet restore "$PSScriptRoot/starter/Starter.csproj"
dotnet run --project "$PSScriptRoot/starter/Starter.csproj" -- reproduce
if ($LASTEXITCODE -ne 0) { throw "Reproduction contract failed with exit code $LASTEXITCODE" }
