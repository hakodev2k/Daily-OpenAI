$ErrorActionPreference='Stop'
dotnet restore (Join-Path $PSScriptRoot '../starter/BuoyStream.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Restore failed' }
