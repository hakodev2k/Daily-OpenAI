$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/TelemetryLab.csproj'
dotnet run --project $project -- reproduce
if ($LASTEXITCODE -ne 0) { throw 'Không tái hiện được symptom; kiểm tra starter chưa sửa.' }
Write-Host 'Reproduction confirmed: receiver rejected at least one frame.'
