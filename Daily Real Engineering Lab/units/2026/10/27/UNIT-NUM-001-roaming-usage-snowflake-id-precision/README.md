# UNIT-NUM-001 — Roaming Usage: Hai Snowflake Event ID bị gộp thành một

## Mục tiêu
Tái hiện sai lệch đối soát khi nhập JSON từ đối tác, thu thập bằng chứng về identity và thiết kế kiểm thử chống hồi quy.

## Bối cảnh thực tế
Hệ thống clearing nhận các roaming usage events của nhà mạng đối tác. Mỗi event có `eventId` kiểu số nguyên 64-bit, `units` là số đơn vị tính cước. Bản ghi gửi lại với cùng ID chỉ được tính một lần. Đối tác báo có hai event hợp lệ nhưng báo cáo nội bộ chỉ ghi nhận một, dù JSON hợp lệ và không có exception.

## Bạn cần làm gì
1. Chạy starter và xác nhận sai lệch ở cả event count, tổng units và danh sách ID.
2. Ghi hypothesis, kiểm tra ranh giới dữ liệu từ JSON tới deduplication.
3. Sửa **chỉ** `starter/UsageImporter.cs`; giữ nguyên contract và behavior với duplicate thực.
4. Chạy verification cho cả ID nhỏ và ID lớn; giải thích tại sao các fixture cũ không phát hiện lỗi.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 5.1+ hoặc 7+. Không cần Docker, database, NuGet bên ngoài hay tài khoản Azure.

## Chạy nhanh
Từ thư mục unit:
```powershell
dotnet --version
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Script chỉ thành công khi starter **thực sự** phát ra `CHECK_FAILED` với exit code dự kiến; lỗi build không được xem là reproduce thành công.

## Những gì cần quan sát
- Số event unique và tổng units được báo cáo so với fixture của đối tác.
- Các ID sau khi đi qua import và trước bước deduplication.
- Tính ổn định của các test với ID nhỏ so với ID 64-bit lớn.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong `starter/`.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ mở [solution/README.md](solution/README.md) sau khi đã tự reproduce, điều tra và thử fix.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). Kết quả đúng: 2 event unique, 70 units trong fixture chính; duplicate thật không được tính hai lần.

## Estimated Time
35 phút. Mức D3 / L1. Validation mode: **static-review** (chưa thực thi trong môi trường tạo lab).
