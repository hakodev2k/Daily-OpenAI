# Chạy từ bất kỳ working directory nào; chứng minh trạng thái starter có lỗi.
$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/LeaseLab.csproj'
dotnet run --project $project --configuration Release -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Reproduction gate failed: expected original starter symptom was not observed.' }
