$ErrorActionPreference = 'Stop'
dotnet --version
if ($LASTEXITCODE -ne 0) { throw 'dotnet SDK unavailable' }
dotnet restore "$PSScriptRoot/../starter/AquariumGate.csproj"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
