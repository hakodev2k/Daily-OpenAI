$ErrorActionPreference = 'Stop'
$unit = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
foreach ($project in @('starter/DispatchLab.csproj', 'solution/DispatchLab.Reference.csproj')) {
    & dotnet restore (Join-Path $unit $project)
    if ($LASTEXITCODE -ne 0) { throw "Restore failed: $project" }
    & dotnet build (Join-Path $unit $project) --configuration Release --no-restore
    if ($LASTEXITCODE -ne 0) { throw "Build failed: $project" }
}
Write-Host 'Setup completed. Both projects build successfully.'
