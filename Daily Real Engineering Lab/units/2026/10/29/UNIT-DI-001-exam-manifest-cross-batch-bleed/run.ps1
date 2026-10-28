$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ExamManifestLab.csproj'
& dotnet run --project $project -- run
if ($LASTEXITCODE -ne 0) {
    throw 'run contract failed. See console output.'
}
