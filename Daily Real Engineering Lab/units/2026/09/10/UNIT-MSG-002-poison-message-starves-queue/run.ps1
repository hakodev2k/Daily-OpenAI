$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/NotificationQueueLab/NotificationQueueLab.csproj'
dotnet run --project $project
