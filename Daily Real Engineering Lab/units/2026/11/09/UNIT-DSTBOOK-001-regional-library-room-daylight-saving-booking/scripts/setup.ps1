$ErrorActionPreference = 'Stop'
dotnet --version
dotnet restore (Join-Path $PSScriptRoot '../starter/DstBooking.csproj')
if ($LASTEXITCODE -ne 0) { throw 'restore failed' }
