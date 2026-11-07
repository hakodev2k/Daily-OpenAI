$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "../starter/MetricCardinalityLab.csproj"
dotnet run --project $project
