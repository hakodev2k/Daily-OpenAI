# Reference Solution — spoiler

## Root cause
Hệ thống dùng UTC offset mùa đông như quy tắc cố định cho mọi ngày. Offset chỉ là chênh lệch ở một thời điểm; timezone chứa quy tắc lịch sử/chuyển mùa. Tháng 7 cảng dùng UTC+03, nhưng starter vẫn dùng UTC+02, khiến UTC được lưu muộn một giờ.

## Sửa
`solution/Program.cs` dùng `TimeZoneInfo.ConvertTimeToUtc` với timezone rõ ràng, và từ chối local time không tồn tại hoặc bị trùng để yêu cầu khách hàng xác định offset/instant. Đừng tự chọn một nhánh trong giờ bị trùng.

## Verify
```powershell
./reproduce.ps1  # starter chưa sửa: chứng minh lỗi
./verify.ps1     # sau khi sửa starter: cả hai trường hợp đúng
dotnet run --project solution/FerrySchedule.csproj
```

## Wrong fixes
- Cộng/trừ một giờ thủ công: sai ở mùa còn lại và khi quy tắc đổi.
- Lưu `DateTime.Now` của máy chủ: phụ thuộc timezone của host.
- Bắt mọi lỗi và dùng UTC mặc định: che giấu vé sai giờ.
- Dùng `DateTimeOffset` với một offset cố định: vẫn không có quy tắc cho ngày tương lai.

## Trade-offs
Dùng timezone ID thật của cảng trong production, có mapping Windows/IANA khi cần; lưu cả local schedule, timezone ID và UTC instant đã tính. Khi luật timezone thay đổi, cần chính sách tái tính lịch tương lai và thông báo khách. Tại giờ chuyển mùa, có thể yêu cầu offset cụ thể thay vì từ chối nếu nghiệp vụ hỗ trợ.

## Verification evidence
Trường hợp 15/02 10:00 -> 08:00Z; 15/07 10:00 -> 07:00Z. Regression kiểm tra cả hai. Static review trong lần tạo lab; chưa chạy SDK thực tế.
