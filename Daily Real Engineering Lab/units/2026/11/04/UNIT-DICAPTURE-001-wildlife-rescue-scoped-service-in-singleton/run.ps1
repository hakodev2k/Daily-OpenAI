$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/Lab.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
