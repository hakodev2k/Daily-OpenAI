$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/AlertDispatch.csproj') -- --verify
exit $LASTEXITCODE
