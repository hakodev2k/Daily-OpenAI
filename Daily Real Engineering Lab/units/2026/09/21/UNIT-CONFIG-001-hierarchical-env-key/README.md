# UNIT-CONFIG-001 — Hierarchical Environment Key

## Mục tiêu
Điều tra vì sao một cấu hình override hoạt động không đúng sau khi chuyển service sang môi trường Linux/container dù application vẫn khởi động bình thường.

## Bối cảnh thực tế
Payment API có giá trị timeout mặc định trong appsettings. Deployment pipeline đặt environment variable để tăng timeout cho một downstream chậm hơn. Release thành công nhưng runtime vẫn dùng giá trị mặc định.

## Bạn cần làm gì
Chạy starter, xác nhận triệu chứng, xác định boundary cấu hình bị sai và sửa starter để giá trị deployment override được bind đúng mà không hard-code giá trị vào code.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell 7+ cho các script tiện ích

## Chạy nhanh
Chạy `./run.ps1`.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script đặt cấu hình giống deployment rồi chạy starter.

## Những gì cần quan sát
- Giá trị environment variable được đặt.
- Giá trị cuối cùng mà application đọc được.
- Cấu trúc key trong configuration hierarchy.

## Quy tắc làm lab
Chỉ sửa trong `starter/`. Không đổi default để làm test pass và không đọc environment variable trực tiếp từ business code.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ mở sau khi tự điều tra](solution/README.md)

## Expected Results
Sau khi sửa, `./verify.ps1` phải báo PASS và runtime nhận đúng deployment override.

## Estimated Time
35 phút.
