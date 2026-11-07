# UNIT-VTASK-001 — Tín hiệu điều độ hợp lệ nhưng thất bại tại bước ghi audit

## Mục tiêu
Điều tra một lỗi production trong pipeline xử lý tín hiệu điều độ, phân biệt lỗi nguồn dữ liệu với lỗi vòng đời kết quả bất đồng bộ, sửa code và chứng minh tính đúng đắn bằng regression checks.

## Bối cảnh thực tế
Hệ thống điều độ lưới điện vùng nhận kết quả kiểm tra công suất từ một gateway tối ưu allocation. Một bản cập nhật tối ưu phân bổ bộ nhớ giúp giảm allocations, nhưng sau đó một số yêu cầu hợp lệ bị trả lỗi ngay trước khi tạo receipt; dashboard báo gateway đã trả kết quả, còn audit lại không có bản ghi tương ứng.

## Bạn cần làm gì
1. Chạy baseline, ghi lại exception, số lần gọi gateway và số audit event.
2. Đề xuất ít nhất hai giả thuyết (gateway trả lỗi, audit ghi lỗi, vòng đời kết quả bất đồng bộ, v.v.) và đối chiếu evidence.
3. Chỉ sửa `starter/TelemetryDispatch.cs`; không sửa test harness, client giả lập hay hợp đồng audit.
4. Chạy `verify.ps1`, bảo đảm cả hai tín hiệu được xử lý, mỗi tín hiệu chỉ gọi gateway một lần và có đúng một audit event.
5. So sánh với reference solution và đánh giá trade-offs.

## Yêu cầu môi trường
.NET SDK 8+ build được `net8.0`, PowerShell 7 hoặc Windows PowerShell 5.1. Không cần NuGet package ngoài, database, Docker, Azure hay API key.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
`REPRODUCTION_CONFIRMED` nghĩa là triệu chứng baseline được tái hiện; không phải kết quả verify.

## Những gì cần quan sát
- Gateway trả một giá trị hợp lệ hay thất bại trước đó?
- Lỗi xảy ra ở giai đoạn nào của pipeline?
- Một request đã tạo bao nhiêu audit event?
- Tần suất gọi gateway có thay đổi sau khi thử fix?
- Những bằng chứng nào giúp loại trừ giả thuyết gateway timeout?

Đọc [incident evidence](evidence/incident.md) và ghi chép vào [workspace](workspace/my-investigation.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ mở sau khi tự điều tra:** [giải thích](solution/README.md) · [reference code](solution/TelemetryDispatch.cs).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
55 phút · L4 / D4 · Validation: **static-review** (chưa thực thi .NET build/tests khi tạo unit).
