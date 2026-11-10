$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/Lab.csproj') --no-launch-profile
exit $LASTEXITCODE
