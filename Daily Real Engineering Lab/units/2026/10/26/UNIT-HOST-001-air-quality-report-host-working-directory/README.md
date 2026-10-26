# UNIT-HOST-001 — Nightly Air-Quality Report: deploy thành công nhưng scheduler báo lỗi

## Mục tiêu
Điều tra một lỗi chỉ xuất hiện khi cùng binary .NET được khởi chạy từ một môi trường vận hành khác. Rèn luyện thu thập bằng chứng, loại trừ giả thuyết và thiết kế regression test cho deployment.

## Bối cảnh thực tế
Một worker tổng hợp chỉ số AQI cho trạm quan trắc chạy đúng khi kiểm thử thủ công. Sau khi chuyển sang scheduler, job thoát lỗi khi tạo báo cáo. Build artifact không thay đổi, health-check vẫn xanh.

## Bạn cần làm gì
1. Tái hiện hai cách khởi chạy cùng một assembly.
2. Ghi ba giả thuyết: thiếu file trong artifact, permission, launch environment.
3. Đối chiếu exception, exit code, vị trí file và diagnostic.
4. Chỉ sửa mã trong `starter/ReportWorker/`; không sửa gate test.
5. Chạy `verify.ps1` và giải thích regression invariant.

## Yêu cầu môi trường
.NET 8 SDK, PowerShell 5.1 hoặc 7+. Không cần Docker, cloud, database, NuGet package hay credentials. SDK được cấu hình trong `global.json`.

## Chạy nhanh
Tại thư mục unit:

```powershell
./run.ps1
./reproduce.ps1
# Sau khi điều tra, sửa starter/ReportWorker/
./verify.ps1
```

Trên Windows PowerShell có thể dùng `.\reproduce.ps1`; tuân thủ execution policy của máy.

## Cách reproduce vấn đề
`./reproduce.ps1` build một lần và khởi chạy **cùng DLL** trong hai launch context. Script exit 0 khi tái hiện được sự khác biệt; đây là phép thử của starter ban đầu, không phải phép thử fix.

## Những gì cần quan sát
So sánh `job_start`, exception, stdout, exit code và artifact giữa hai lần chạy. Đọc [synthetic incident evidence](evidence/incident.log) và xác định log nào là nhiễu. Kết quả báo cáo đúng: `Station NorthStation: AQI 42.`.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace](workspace/my-investigation.md).
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
⚠️ **Spoiler**: [Reference explanation](solution/README.md) và [source](solution/FileTemplateProvider.cs). Chỉ mở sau khi tự sửa.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
55 phút · L3 / D4.
