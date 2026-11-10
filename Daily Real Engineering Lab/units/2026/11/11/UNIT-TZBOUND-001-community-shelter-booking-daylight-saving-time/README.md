# UNIT-TZBOUND-001 — Đêm đổi giờ tại nhà tạm trú

## Mục tiêu
Điều tra sai lệch thời lượng lưu trú, refactor hàm tính giờ và viết regression evidence.

## Bối cảnh thực tế
Báo cáo công suất giường ở một cơ sở New York sai lệch với nhật ký check-in/check-out vào một số đêm. Dữ liệu đầu vào là timestamp ISO-8601 có offset; các ca bình thường vẫn đúng.

## Bạn cần làm gì
Chạy chương trình, ghi lại các timestamp, actual và expected; đề xuất hai hypothesis, sửa duy nhất `starter/StayDuration.cs`, xác minh lại.

## Yêu cầu môi trường
.NET SDK 8.0+, PowerShell, không cần cloud hoặc dịch vụ ngoài.

## Chạy nhanh
```powershell
./scripts/reproduce.ps1
./scripts/verify.ps1
```

## Cách reproduce vấn đề
Chạy reproduce từ thư mục unit khi starter chưa sửa. Script phải nhận diện được ca lỗi và ca đối chứng.

## Những gì cần quan sát
Timestamp, offset, chênh lệch giờ thực tế, mã thoát của test harness; không suy luận nguyên nhân chỉ từ thông báo lỗi.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[01](hints/hint-01.md) · [02](hints/hint-02.md) · [03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp](solution/README.md)

## Expected Results
Trước: ca đặc biệt 8 giờ, hợp đồng yêu cầu 9; ca đối chứng 8 giờ. Sau: 9 và 8 giờ, từ chối checkout trước checkin.

## Estimated Time
65 phút.
