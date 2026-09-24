$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/ParallelFixtureLab.csproj"
if ($LASTEXITCODE -eq 0) { throw 'Expected the starter to expose at least one parallel-suite failure.' }
Write-Host 'Reproduction confirmed.'