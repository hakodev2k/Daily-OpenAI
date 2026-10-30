# UNIT-DISP-001 — Sự cố lưu hồ sơ kiểm định tàu ven biển

## Mục tiêu
Điều tra sự cố có nhiều tín hiệu lỗi trong cùng một lần xử lý, thu thập bằng chứng và thiết kế hợp đồng chẩn đoán không làm mất nguyên nhân quan trọng.

## Bối cảnh thực tế
Một dịch vụ .NET xuất hồ sơ kiểm định tàu lên kho lưu trữ. Trong đợt lỗi, telemetry của object storage báo upload bị từ chối, nhưng bản ghi incident do worker phát ra chỉ có một lỗi liên quan tới kết thúc phiên xuất. Đội vận hành có thể xử lý sai nguyên nhân nếu chỉ dựa vào lỗi cuối.

## Bạn cần làm gì
Reproduce, ghi ít nhất hai giả thuyết, kiểm tra luồng ngoại lệ và số lần gọi từng pha. Sửa **chỉ** `ArchiveExporter.ExportAsync` trong `starter/Program.cs` để cả lỗi chính lẫn lỗi dọn dẹp được chẩn đoán khi chúng cùng xảy ra, không gọi pha nào quá một lần. Không sửa fixture kiểm thử.

## Yêu cầu môi trường
.NET 8 SDK, PowerShell 5.1+ hoặc PowerShell 7. Không cần Docker, database, Azure account hay NuGet bên ngoài.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` trước khi sửa: script exit 0 và in `REPRODUCED` nếu triệu chứng gốc tồn tại. `./verify.ps1` phải exit khác 0 trên starter ban đầu.

## Những gì cần quan sát
So sánh telemetry của pha upload và pha kết thúc, exception mà caller thực sự bắt được, stack trace, và số lần mỗi pha chạy trong bốn tình huống: hai lỗi, chỉ upload lỗi, chỉ kết thúc lỗi, không lỗi. Ghi kết quả vào [workspace](workspace/my-investigation.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và bằng chứng.
3. Thử fix trong starter.
4. Verify trên starter đã sửa.
5. Sau đó mới mở reference solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [giải thích](solution/README.md) · [reference code](solution/Program.cs). Không mở trước khi tự sửa.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
50 phút · L2 / D4. Validation: **static-review**; chưa chạy .NET trong môi trường tạo lab.
