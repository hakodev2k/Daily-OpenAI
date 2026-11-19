$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "../starter/CargoManifestLab.csproj"
dotnet run --project $project -- --reproduce
if ($LASTEXITCODE -ne 0) { throw "Reproduction did not show expected symptom." }
