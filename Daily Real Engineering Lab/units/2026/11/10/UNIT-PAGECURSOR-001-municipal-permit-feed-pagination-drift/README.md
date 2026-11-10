# UNIT-PAGECURSOR-001 — Phân trang hồ sơ cấp phép khi dữ liệu thay đổi

## Mục tiêu
Đánh giá tính nhất quán của API phân trang dưới thao tác ghi đồng thời, thu thập bằng chứng và đề xuất hợp đồng phân trang có thể kiểm chứng.

## Bối cảnh thực tế
Cổng thông tin giấy phép xây dựng cho phép cán bộ duyệt lần lượt từng trang 2 hồ sơ, mới nhất trước. Hồ sơ mới vẫn được nhập trong lúc cán bộ duyệt.

## Bạn cần làm gì
1. Chạy starter, ghi lại ID từng trang và kiểm tra ID trùng hoặc bị bỏ qua.
2. Viết ít nhất hai giả thuyết trước khi xem hints.
3. Sửa phần `PermitFeed.Pages` để hợp đồng phân trang ổn định với khóa sắp xếp xác định.
4. Giải thích khác biệt giữa feed cập nhật liên tục và phiên duyệt có snapshot; nêu trade-off của cursor và OFFSET.
5. Chạy `verify.ps1`, lưu bằng chứng trong `workspace/notes.md`.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+; không cần DB hoặc Internet. Fixture mô phỏng một bảng dữ liệu có thứ tự xác định.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy script, quan sát trang đầu; hệ thống nhập một hồ sơ mới trước khi tải trang tiếp theo. Đối chiếu danh sách các hồ sơ vốn tồn tại trước thao tác nhập.

## Những gì cần quan sát
Ghi lại các ID trả về, ID ban đầu, số lần xuất hiện và tính ổn định khi hai hồ sơ có cùng thời điểm. Đừng kết luận chỉ dựa vào tổng số kết quả.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 0](hints/00.md) · [Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md)

## Reference Solution
⚠️ Spoiler: [Giải pháp và trade-offs](reference/solution.md).

## Expected Results
Tất cả hồ sơ có mặt ở thời điểm bắt đầu phiên duyệt xuất hiện đúng một lần, theo thứ tự nhất quán; hồ sơ nhập mới không làm trôi phiên duyệt hiện tại. Script verification phải báo PASS.

## Estimated Time
50 phút.
