$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Không reproduce được expected cold-key burst. Reset starter nếu đã sửa code.' }
Write-Host 'REPRODUCED: functional output đúng nhưng một cold-key burst tạo nhiều downstream loads.'