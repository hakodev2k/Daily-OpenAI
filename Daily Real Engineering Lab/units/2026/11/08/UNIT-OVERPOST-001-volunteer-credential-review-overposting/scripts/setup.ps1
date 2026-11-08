$ErrorActionPreference='Stop'
dotnet restore (Join-Path $PSScriptRoot '../starter/VolunteerLab.csproj')
exit $LASTEXITCODE
