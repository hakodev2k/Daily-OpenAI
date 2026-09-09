$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $root

dotnet restore ./starter/StreamUploadLab.csproj
if ($LASTEXITCODE -ne 0) { throw 'dotnet restore failed' }

dotnet build ./starter/StreamUploadLab.csproj --no-restore
if ($LASTEXITCODE -ne 0) { throw 'dotnet build failed' }

dotnet run --project ./starter/StreamUploadLab.csproj --no-build -- verify
if ($LASTEXITCODE -ne 0) { throw 'verification failed' }
