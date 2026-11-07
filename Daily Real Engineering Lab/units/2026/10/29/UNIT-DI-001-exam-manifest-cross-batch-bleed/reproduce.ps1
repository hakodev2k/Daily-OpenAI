$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ExamManifestLab.csproj'
& dotnet run --project $project -- reproduce
if ($LASTEXITCODE -ne 0) {
    throw 'reproduce contract failed. See console output.'
}
