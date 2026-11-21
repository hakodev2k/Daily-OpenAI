$ErrorActionPreference='Stop'
dotnet restore (Join-Path $PSScriptRoot '../starter/IntakeLab.csproj')
exit $LASTEXITCODE
