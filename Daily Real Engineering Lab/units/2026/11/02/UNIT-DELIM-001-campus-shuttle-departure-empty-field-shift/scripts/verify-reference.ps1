$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../solution/Solution.csproj'
& dotnet run --project $project -- --check
if ($LASTEXITCODE -ne 0) { throw 'Reference solution failed behavior contracts.' }
Write-Host 'REFERENCE VERIFIED.'
