# UNIT-READLOOP-001 — Khung telemetry phao cảng bị cắt khi đường truyền phân mảnh

## Mục tiêu
Điều tra lỗi đọc dữ liệu nhị phân qua `Stream` khi dữ liệu đến thành nhiều đợt; xây dựng kiểm thử cho dữ liệu đầy đủ, phân mảnh và bị ngắt.

## Bối cảnh thực tế
Gateway của cảng nhận frame độ cao sóng từ phao qua kết nối TCP. Với mô phỏng nội bộ, frame luôn hợp lệ; trên đường truyền thực tế, đôi khi gateway trả về giá trị 0 hoặc số đo sai dù sender gửi đủ byte.

## Bạn cần làm gì
Reproduce; ghi ít nhất hai hypothesis; thu thập byte count; sửa `starter/Program.cs` để trả đúng payload, không chấp nhận frame thiếu, và không thay đổi wire format.

## Yêu cầu môi trường
.NET SDK 8 hoặc mới hơn, PowerShell 7+; không cần mạng, Docker hoặc NuGet ngoài SDK.

## Chạy nhanh
Từ thư mục unit:
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
`./reproduce.ps1` chạy bản starter chưa sửa trên frame hợp lệ bị chia nhỏ và kỳ vọng chứng minh sai lệch.

## Những gì cần quan sát
So sánh số byte đã được sender cung cấp, số byte consumer nhận ở mỗi lần đọc và nội dung payload. Lỗi xuất hiện ngay cả khi không có packet loss.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong `starter/Program.cs`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md)

## Expected Results
Trước sửa: case `fragmented` thất bại; sau sửa: `fragmented`, `single`, `truncated` và `empty` đều PASS. Frame thiếu phải bị từ chối thay vì trả payload giả.

## Estimated Time
40 phút.
