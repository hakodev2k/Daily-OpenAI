# Reference Solution (spoiler)

## Root cause
`DateTimeOffset.DateTime` lấy thành phần local clock và loại bỏ offset. Đêm 2026-11-01 ở New York chuyển -04:00 sang -05:00, nên hai local clocks chênh 8 giờ nhưng hai instant chênh 9 giờ.

## Fix
Trong `starter/StayDuration.cs`, giữ guard hiện tại và thay return bằng:
```csharp
return (decimal)(checkOut - checkIn).TotalHours;
```

## Verification
Chạy `./scripts/verify.ps1` với chính starter đã sửa, không chỉnh test. Ca đối chứng 8 giờ, ca đổi giờ 9 giờ, checkout trước checkin bị từ chối.

## Wrong fixes
- Cộng cố định 1 giờ vào mọi ca tháng 11: sai với ca không đổi offset và vùng khác.
- Dùng `DateTime.Kind=Unspecified`: tiếp tục mất offset.
- Đếm ngày rồi nhân 24: đo lịch dân sự thay vì elapsed hours.

## Trade-offs
Nếu tính phí theo **đêm** hoặc **ngày lịch địa phương**, cần chính sách riêng về time zone, ambiguous/nonexistent times, rounding và version của timezone database. Không được tái sử dụng elapsed hours cho hợp đồng khác.
