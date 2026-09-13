$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '..\starter\DeployPathLab.csproj'
dotnet run --project $project
if ($LASTEXITCODE -ne 0) {
    throw 'Learner solution did not satisfy the expected path contract.'
}
Write-Host 'Verification passed.'
