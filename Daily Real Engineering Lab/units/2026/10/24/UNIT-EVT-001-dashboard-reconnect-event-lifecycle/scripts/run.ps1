$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '..\starter\EventReconnectLab.csproj'
dotnet run --project $project --configuration Release -- smoke
exit $LASTEXITCODE
