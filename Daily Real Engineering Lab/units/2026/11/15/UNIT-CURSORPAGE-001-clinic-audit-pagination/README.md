# UNIT-CURSORPAGE-001 — Audit log bị trùng khi phân trang

## Mục tiêu
Điều tra consistency của API phân trang khi dữ liệu thay đổi giữa các request, rồi refactor contract và kiểm chứng bằng regression test.

## Bối cảnh thực tế
Dashboard phòng khám hiển thị audit events mới nhất. Người vận hành chuyển trang trong lúc hệ thống tiếp tục ghi sự kiện. Có báo cáo rằng một event xuất hiện hai lần, trong khi một event cũ không được nhìn thấy.

## Bạn cần làm gì
Chạy starter, lưu kết quả hai trang, nêu ít nhất hai giả thuyết, sửa API paging và chạy verify trên code bạn sửa. Thiết kế cursor cho MongoDB với sort ổn định và nêu cách xử lý dữ liệu thay đổi.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7 hoặc Windows PowerShell; không cần MongoDB server cho replay.

## Chạy nhanh
`pwsh ./scripts/setup.ps1`
`pwsh ./scripts/reproduce.ps1`

## Cách reproduce vấn đề
Chạy `pwsh ./scripts/reproduce.ps1` từ thư mục unit. Ghi lại thứ tự event ở từng trang.

## Những gì cần quan sát
So sánh tập ID của hai trang, số phần tử duy nhất và chuỗi thứ tự; xác định liệu mọi event của tập dữ liệu ban đầu đều được duyệt đúng một lần hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Xem sau khi tự sửa](solution/README.md).

## Expected Results
[Trước](expected-results/before.md) · [Sau](expected-results/after.md)

## Estimated Time
45 phút.