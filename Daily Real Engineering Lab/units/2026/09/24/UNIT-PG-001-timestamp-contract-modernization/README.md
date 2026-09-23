# UNIT-PG-001 — Modernize timestamp contract without shifting business time

## Mục tiêu

Modernize một luồng lưu/đọc timestamp kiểu legacy sang contract rõ ràng, đồng thời giữ đúng instant nghiệp vụ qua boundary giữa application và database.

## Bối cảnh thực tế

Một worker xử lý lịch giao hàng được chuyển từ Windows VM sang Linux container. Dữ liệu cũ vẫn đọc được, ứng dụng không throw exception, nhưng một số lịch hiển thị lệch giờ khi đi qua persistence boundary. Team muốn chuẩn hóa contract trước khi migration PostgreSQL được rollout rộng hơn.

## Bạn cần làm gì

1. Chạy starter và reproduce triệu chứng.
2. Ghi hypothesis trước khi sửa.
3. Xác định boundary nào đang làm mất hoặc diễn giải sai thông tin thời gian.
4. Refactor starter sao cho cùng một business instant được bảo toàn qua round-trip.
5. Chạy `verify.ps1` trên chính code bạn đã sửa.
6. Sau đó mới so sánh với reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy starter với một timestamp đại diện cho lịch giao hàng và kiểm tra round-trip qua persistence adapter mô phỏng contract database.

## Những gì cần quan sát

- Giá trị instant trước và sau round-trip có còn đại diện cùng một thời điểm hay không.
- `DateTime.Kind` tại các boundary.
- Offset/UTC representation trước khi ghi và sau khi đọc.
- Ứng dụng vẫn có thể chạy thành công dù dữ liệu thời gian bị dịch chuyển.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

Không có hint trực tiếp trong README. Hãy ưu tiên quan sát `Kind`, UTC instant và contract của persistence adapter.

## Reference Solution

Xem `solution/README.md` chỉ sau khi đã thử fix.

## Expected Results

Before: round-trip hoàn tất nhưng assertion về business instant thất bại.

After: round-trip bảo toàn cùng một UTC instant và contract thời gian trở nên explicit.

## Estimated Time

45–60 phút.