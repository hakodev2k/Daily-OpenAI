$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ExamManifestLab.csproj'
& dotnet run --project $project -- verify
if ($LASTEXITCODE -ne 0) {
    throw 'verify contract failed. See console output.'
}
