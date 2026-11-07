# UNIT-HTTPRANGE-001 — Audio archive tải tiếp bị hỏng

## Mục tiêu
Điều tra lỗi integrity của chức năng resume download khi mạng không ổn định; phân biệt kết quả download thành công về HTTP với dữ liệu file đúng.

## Bối cảnh thực tế
Kho lưu trữ lịch sử truyền khẩu cho phép tải file âm thanh lớn. Sau khi mất mạng, người dùng resume download. Một số file có dung lượng bất thường và checksum không khớp dù request không báo lỗi.

## Bạn cần làm gì
1. Chạy mô phỏng hai kiểu phản hồi của server.
2. Ghi nhận status, response headers, kích thước file và SHA-256.
3. Nêu ít nhất hai giả thuyết trước khi sửa starter.
4. Sửa code tại `starter/Program.cs` để cả hai chế độ đều tạo file nguyên vẹn.

## Yêu cầu môi trường
.NET SDK 8; PowerShell; không cần internet hoặc NuGet ngoài SDK.

## Chạy nhanh
```powershell
pwsh ./reproduce.ps1
pwsh ./verify.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`; script kiểm tra hiện tượng với server giả lập tại chỗ và trả về thành công khi đã quan sát được lỗi mong đợi.

## Những gì cần quan sát
Ghi status HTTP, header Content-Range (nếu có), số byte đã có trước request, số byte sau request, SHA-256. Đối chiếu giữa hai chế độ server.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ xem [lời giải](solution/README.md) sau khi tự thử.

## Expected Results
Trước sửa: một trường hợp resume làm checksum sai. Sau sửa: cả hai trường hợp tạo file khớp dữ liệu gốc, không nhân đôi byte.

## Estimated Time
40 phút.
