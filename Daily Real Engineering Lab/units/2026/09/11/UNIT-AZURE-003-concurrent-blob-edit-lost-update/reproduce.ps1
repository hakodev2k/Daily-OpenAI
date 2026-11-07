param()
$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Lab.csproj'

dotnet run --project $project -- reproduce
if ($LASTEXITCODE -ne 0) {
    throw 'Starter symptom was not reproduced.'
}

Write-Host 'Reproduction contract satisfied.'
