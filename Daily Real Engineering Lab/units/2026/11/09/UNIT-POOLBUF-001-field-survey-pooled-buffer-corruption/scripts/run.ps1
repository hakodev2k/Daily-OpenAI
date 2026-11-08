$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/ThumbnailLab.csproj'
dotnet run --project $project --configuration Release
exit $LASTEXITCODE
