# UNIT-CHAN-001 — Hàng đợi nhập kho tăng không giới hạn khi downstream chậm

## Mục tiêu
Điều tra một pipeline producer/consumer trong .NET, thu thập bằng chứng về khả năng tiếp nhận công việc khi downstream chậm và bảo đảm không thất lạc bản ghi sau khi khắc phục.

## Bối cảnh thực tế
Một dịch vụ đồng bộ tồn kho nhận hàng loạt sự kiện từ các chi nhánh. Khi hệ thống đối soát phía sau tạm ngừng xử lý, API tiếp nhận vẫn báo nhận nhanh. Sau đó bộ nhớ tăng mạnh theo kích thước burst và thời gian hoàn thành backlog kéo dài. Nhóm vận hành cần một giới hạn tài nguyên rõ ràng mà không được âm thầm làm mất sự kiện.

## Bạn cần làm gì
1. Chạy starter và ghi lại các chỉ số trước/sau khi downstream được mở lại.
2. Phân biệt số sự kiện đã được tiếp nhận với số đã hoàn tất.
3. Đề xuất ít nhất hai giả thuyết; kiểm tra giả thuyết bằng code và output.
4. Sửa **chỉ** `starter/DispatchQueue.cs` để kiểm soát backlog mà không làm mất dữ liệu.
5. Chạy verify trên starter đã sửa; sau đó mới mở reference solution.

## Yêu cầu môi trường
- .NET 8 SDK, PowerShell 7+ hoặc Windows PowerShell 5.1.
- Không cần Docker, Redis, Azure hay NuGet bên ngoài.

## Chạy nhanh
Từ thư mục lab:
```powershell
./setup.ps1
./run.ps1
```
Nếu chính sách PowerShell ngăn chạy script, dùng `dotnet run --project starter/QueueLab.Starter.csproj -- reproduce`.

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Script thành công (exit code 0) khi quan sát được triệu chứng của **starter gốc**; đây chưa phải xác nhận fix. Sau khi sửa, chạy `./verify.ps1` (không chạy lại reproduce để đánh giá fix).

## Những gì cần quan sát
- `AcceptedBeforeRelease` và `AllPublishedBeforeRelease` khi downstream chưa xử lý xong sự kiện đầu tiên.
- `PeakOutstanding` trong toàn bộ burst.
- `Accepted`, `Completed` và `AllMessagesProcessedExactlyOnce` khi pipeline kết thúc.
- Phân biệt throughput, backlog, thời gian chờ producer và mất dữ liệu.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: chỉ mở [Reference Solution](solution/README.md) sau khi đã tự sửa và verify.

## Expected Results
- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time
55 phút. Ghi lại suy luận trong [workspace](workspace/my-investigation.md).
