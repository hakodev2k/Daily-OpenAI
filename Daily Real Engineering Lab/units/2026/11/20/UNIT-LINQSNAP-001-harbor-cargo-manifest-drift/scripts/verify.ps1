$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "../starter/CargoManifestLab.csproj"
dotnet run --project $project -- --verify
if ($LASTEXITCODE -ne 0) { throw "Learner implementation did not satisfy the manifest contract." }
