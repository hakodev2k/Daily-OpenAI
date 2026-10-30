$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ShelterQuotaLab.csproj'
dotnet --version
if ($LASTEXITCODE -ne 0) { throw 'Required .NET SDK not found.' }
dotnet restore $project
if ($LASTEXITCODE -ne 0) { throw 'Restore failed.' }
dotnet build $project --configuration Release --no-restore
exit $LASTEXITCODE
