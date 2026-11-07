$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "../starter/Lab.csproj"
dotnet restore $project
dotnet run --project $project
