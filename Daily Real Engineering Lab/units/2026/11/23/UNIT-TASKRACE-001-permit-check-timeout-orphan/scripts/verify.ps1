$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/PermitDeadlineLab.csproj'
dotnet run --project $project -- verify
if ($LASTEXITCODE -ne 0) { exit 1 }
