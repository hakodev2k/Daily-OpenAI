$ErrorActionPreference = 'Stop'
dotnet run --project (Join-Path (Split-Path $PSScriptRoot -Parent) 'starter/ProbeLab.csproj')
