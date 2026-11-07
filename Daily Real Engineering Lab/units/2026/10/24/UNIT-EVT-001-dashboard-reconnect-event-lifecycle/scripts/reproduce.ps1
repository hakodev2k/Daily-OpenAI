$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '..\starter\EventReconnectLab.csproj'
dotnet run --project $project --configuration Release -- reproduce
exit $LASTEXITCODE
