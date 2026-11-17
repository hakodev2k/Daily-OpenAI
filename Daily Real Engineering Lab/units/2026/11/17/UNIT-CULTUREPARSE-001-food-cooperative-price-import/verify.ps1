$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PriceImport.csproj'
dotnet run --project $project -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Learner code did not satisfy regression contract' }
Write-Host 'Verification passed against learner-editable starter.'
