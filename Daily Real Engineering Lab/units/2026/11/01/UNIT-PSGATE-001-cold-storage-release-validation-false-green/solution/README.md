# Reference Solution — chỉ xem sau khi reproduce và tự thử sửa

## Symptoms
Validator phát hiện 2 báo cáo nhiệt độ sai contract, nhưng wrapper vẫn in thông báo pass và pipeline coi gate thành công.

## Evidence
`evidence/incident.log` cho thấy `VALIDATOR_FAIL`, tiếp theo `QUALITY_GATE_PASS`, rồi `child_exit_code=0`. `scripts/reproduce.ps1` chứng minh sai lệch tương tự bằng process con thật.

## Root cause
`starter/invoke-quality-gate.ps1` gọi native `.cmd` rồi tiếp tục chạy `Write-Output` mà không kiểm tra `$LASTEXITCODE`. Nonzero exit status của native command không mặc nhiên được biến thành lỗi kết thúc script ở cấu hình đang dùng; lệnh PowerShell cuối thành công khiến process script kết thúc thành công.

## Why the fix works
[Reference code](invoke-quality-gate.ps1) lấy `$LASTEXITCODE` **ngay sau** native command, tách hai nhánh, không in pass khi lỗi và `exit` bằng đúng mã lỗi validator. Nhánh pass vẫn `exit 0`.

## How to verify
Copy nội dung reference vào `starter/invoke-quality-gate.ps1` (hoặc tự triển khai tương đương) rồi chạy `scripts/verify.ps1`. Harness luôn gọi **starter**, không gọi reference. Kiểm tra thêm cả `pass` và `fail` trong process PowerShell độc lập.

## Alternative fixes
- `if ($LASTEXITCODE -ne 0) { throw ... }` phù hợp khi chỉ cần nonzero, nhưng thường trả mã process 1 thay vì giữ nguyên mã native.
- PowerShell 7.4+ có cơ chế `$PSNativeCommandUseErrorActionPreference` tùy cấu hình; không nên phụ thuộc vào thiết lập máy agent để xác định kết quả release. Nên quy định và kiểm thử hợp đồng exit code tường minh.

## Wrong / tempting fixes
- Chỉ tìm chuỗi `FAIL` trong log: có thể báo sai khi output thay đổi hoặc chứa chuỗi trong dữ liệu.
- Đặt `$ErrorActionPreference='Stop'` rồi giả định mọi native nonzero đều dừng script: không đủ trong cấu hình được mô phỏng.
- Chỉ đổi màu UI của CI: không sửa exit status thực tế.
- Nuốt mọi lỗi và trả 0 để pipeline không đỏ: gây false-green nghiêm trọng.

## Production implications & trade-offs
Mã lỗi cần được bảo toàn hoặc ánh xạ có chủ đích; thông báo log và trạng thái orchestrator phải nhất quán. Với nhiều native steps, kiểm tra exit code **sau từng step**, không chỉ ở cuối pipeline. Cần contract test cho cả failure và success trước khi nâng cấp PowerShell hoặc agent.

## What a Senior engineer should notice
Release gate là một ranh giới tin cậy giữa process con và orchestrator. Bằng chứng về kết quả kiểm tra phải đi qua ranh giới đó bằng status được máy hiểu, không chỉ bằng console text. Phải test negative path, không chỉ happy path.
