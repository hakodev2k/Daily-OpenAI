# UNIT-ETAG-001 — Garden Plot API: Conditional Update Mất Dữ Liệu

## Mục tiêu
Điều tra và sửa hợp đồng cập nhật có điều kiện của API, bảo đảm hai người quản lý không ghi đè thay đổi của nhau.

## Bối cảnh thực tế
Hai tình nguyện viên chỉnh sửa phân bổ luống vườn cộng đồng. Dashboard gửi `If-Match` để tránh ghi đè phiên bản mới, nhưng một yêu cầu từ trình duyệt cũ vẫn trả về 200 dù dữ liệu đã thay đổi.

## Bạn cần làm gì
Chạy harness, ghi lại response code và phiên bản trước/sau. Đề xuất ít nhất hai giả thuyết, sửa implementation trong `starter/Program.cs`, rồi chứng minh cả luồng hợp lệ và stale request.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell 5.1. Không cần database hay cloud.

## Chạy nhanh
```powershell
dotnet run --project starter/GardenPlot.csproj
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Script yêu cầu trạng thái lỗi ban đầu; sau khi sửa, script này phải không còn báo reproduce thành công.

## Những gì cần quan sát
So sánh mã HTTP, `ETag`, phiên bản lưu trữ và tên người nhận luống qua từng thao tác. Xác định điều kiện nào cho phép ghi đè.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp và trade-offs](solution/README.md)

## Expected Results
Ban đầu: harness ghi nhận stale write được chấp nhận. Sau fix: tất cả kiểm tra trong `verify.ps1` thành công, không làm hỏng update hợp lệ.

## Estimated Time
65 phút.
