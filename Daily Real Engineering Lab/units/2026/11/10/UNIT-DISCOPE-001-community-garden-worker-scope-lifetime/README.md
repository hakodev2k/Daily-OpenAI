# UNIT-DISCOPE-001 — Community garden worker: vòng đời scoped dependency

## Mục tiêu
Refactor một worker chạy dài hạn, xác định ranh giới sở hữu tài nguyên và chứng minh rằng hai batch độc lập đều hoàn tất.

## Bối cảnh thực tế
Một worker nội bộ xử lý hóa đơn nước tưới cho các khu vườn cộng đồng. Batch đầu thành công; batch kế tiếp báo lỗi vòng đời tài nguyên. API quản trị vẫn hoạt động bình thường.

## Bạn cần làm gì
Chạy starter, ghi lại lỗi và thời điểm phát sinh, đề xuất ít nhất hai giả thuyết, sửa **starter/Program.cs**, sau đó verify. Không sửa assertion trong chương trình.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+; không cần Docker, Azure hoặc database.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy script và ghi lại batch nào hoàn tất, batch nào thất bại. Script sẽ báo thành công nếu triệu chứng được tái hiện đúng.

## Những gì cần quan sát
Quan sát số batch đã commit, thời điểm giải phóng tài nguyên, loại exception và hành vi giữa hai lần xử lý. Kiểm tra xem việc chạy một batch riêng lẻ có đủ để kết luận worker đúng hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md)

## Reference Solution
**Spoiler:** chỉ mở [reference/solution.md](reference/solution.md) sau khi tự thử.

## Expected Results
Trước sửa: batch đầu được ghi, batch thứ hai thất bại với lỗi tài nguyên đã giải phóng. Sau sửa: cả hai batch được ghi đúng một lần, không có lỗi và không để lại scope chưa dispose.

## Estimated Time
45 phút.
