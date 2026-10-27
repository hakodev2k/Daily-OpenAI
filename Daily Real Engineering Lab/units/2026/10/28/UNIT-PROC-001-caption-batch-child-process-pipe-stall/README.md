# UNIT-PROC-001 — Pipeline tạo phụ đề treo dù converter vẫn khởi chạy

## Mục tiêu
Điều tra một job .NET gọi chương trình ngoài, thu thập bằng chứng về trạng thái tiến trình, sau đó refactor để giữ đúng hợp đồng I/O, timeout và cleanup. Verify trên code do bạn chỉnh, không chỉ trên reference.

## Bối cảnh thực tế
Nền tảng video giáo dục tạo phụ đề cho bài giảng. Worker chạy converter, lấy phụ đề và diagnostic. Các bản ghi nhỏ thành công; một batch mới có nhiều diagnostic thì job timeout dù CPU thấp và converter đã start. Fixture hoàn toàn synthetic, chạy local.

## Bạn cần làm gì
1. Chạy starter và lưu output, deadline, exit status nếu có.
2. Ghi ba giả thuyết về converter, process lifecycle và I/O.
3. Chỉ sửa `starter/ConversionRunner.cs`; không chỉnh worker, harness hoặc scripts.
4. Bảo toàn cả caption và diagnostic, xử lý timeout/cancellation và cleanup.
5. Chạy `verify.ps1` trước khi đọc reference.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 5.1+ hoặc 7+, lệnh `dotnet` trên PATH. Không cần Docker, cloud hoặc NuGet ngoài SDK.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Exit 0 với `REPRODUCTION_CONFIRMED` xác nhận triệu chứng starter gốc, không xác nhận fix.

## Những gì cần quan sát
- Child đã exit chưa tại thời điểm timeout?
- CPU thấp có chứng minh tiến trình không bị chặn không?
- Output nào được thu trước khi job kết thúc?
- Sau khi sửa, cần kiểm tra những invariant nào ngoài việc không còn timeout?

Đọc [evidence](evidence/incident.md) và ghi hypothesis vào [workspace](workspace/my-investigation.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md) — chỉ mở sau khi tự điều tra.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
70 phút · L4 / D4 · Validation: static-review (chưa thực thi .NET build/tests).
