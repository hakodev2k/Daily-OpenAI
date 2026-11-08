$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot '../starter/VolunteerLab.csproj')
exit $LASTEXITCODE
