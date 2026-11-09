$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/MarineSampleIntake.csproj')
exit $LASTEXITCODE
