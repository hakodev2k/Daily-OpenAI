$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- --verify
if ($LASTEXITCODE -ne 0) { throw 'VERIFY_FAIL: learner-editable starter vẫn tạo nhiều expensive loads cho cùng cold key hoặc functional behavior bị regression.' }
Write-Host 'VERIFY_PASS: concurrent callers nhận đúng dữ liệu và cùng cold key chỉ kích hoạt một expensive load.'