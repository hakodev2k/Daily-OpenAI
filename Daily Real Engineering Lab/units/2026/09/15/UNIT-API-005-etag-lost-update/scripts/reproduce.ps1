$ErrorActionPreference = 'Stop'
$root = Join-Path $PSScriptRoot '..\starter'
dotnet run --project (Join-Path $root 'ApiConcurrencyLab.csproj')
