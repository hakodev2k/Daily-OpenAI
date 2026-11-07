# UNIT-ASP-004 — Exception After Response Started

## Mục tiêu
Điều tra lifecycle của HTTP response trong ASP.NET Core khi lỗi xảy ra giữa một response đang được tạo dở.

## Bối cảnh thực tế
API export báo cáo hoạt động với dataset nhỏ. Với một account cụ thể, client nhận HTTP 200 nhưng file tải về bị thiếu; server đồng thời ghi exception. Team kỳ vọng global exception handler trả JSON 500 nhưng client không bao giờ thấy JSON đó.

## Bạn cần làm gì
Reproduce, thu thập status/body/log evidence, ghi ít nhất ba hypothesis, sửa learner-editable starter, rồi verify cả success path và failure path.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script gọi endpoint với input kích hoạt failure sau khi một phần report đã được xử lý.

## Những gì cần quan sát
- HTTP status mà client nhận.
- Body có hoàn chỉnh hay không.
- Server exception xuất hiện ở thời điểm nào so với response lifecycle.
- Global error handling có còn khả năng thay đổi response hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify.
5. Sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
[Spoiler](solution/README.md)

## Expected Results
Before: failure path có thể tạo success status + incomplete payload thay vì một error response có contract rõ ràng. After: fallible preparation failure được biểu diễn nhất quán trước khi response commit; success path vẫn trả report đầy đủ.

Nếu không reproduce được, xác nhận port 5191 chưa dùng và chạy `dotnet --info`.

## Estimated Time
50 phút.