# UNIT-BUFFEROWN-001 — Gói telemetry bị thay nội dung khi truyền bất đồng bộ

## Mục tiêu
Điều tra sự cố sai lệch dữ liệu trong pipeline C#/.NET bất đồng bộ bằng bằng chứng có thể lặp lại; đề xuất và kiểm chứng ranh giới sở hữu dữ liệu.

## Bối cảnh thực tế
Trạm thu tín hiệu bóng thám không gửi các frame nhiệt độ theo lô lên hệ thống lưu trữ. Khi hai frame được gửi sát nhau, gateway báo thành công nhưng bản ghi đầu tiên đôi khi có nội dung của bản ghi tiếp theo. Không có HTTP 500 hay timeout.

## Bạn cần làm gì
Chạy mô phỏng, ghi lại hai frame nguồn và hai frame nhận, xây dựng ít nhất hai hypothesis, sửa **starter/Program.cs** mà không sửa fake gateway hay các assertion, rồi kiểm tra lại.

## Yêu cầu môi trường
.NET SDK 8.0, PowerShell 7+ hoặc Windows PowerShell; không cần Docker, Azure hay NuGet ngoài SDK.

## Chạy nhanh
Từ thư mục unit: `dotnet run --project starter/TelemetryLab.csproj -- reproduce` hoặc `./scripts/reproduce.ps1`.

## Cách reproduce vấn đề
1. Chạy `./scripts/reproduce.ps1` (exit 0 nghĩa là triệu chứng được tái hiện).
2. Đọc output `SOURCE` và `RECEIVED`, so sánh theo thứ tự hai frame.
3. Xem thứ tự thao tác ở `starter/Program.cs`; gateway giả lập giữ các send đang chờ để loại bỏ yếu tố timing ngẫu nhiên.

## Những gì cần quan sát
- Gateway trả về thành công cho cả hai send.
- Số frame vẫn bằng 2, nhưng nội dung có thể không khớp với dữ liệu nguồn.
- Không có retry, network failure hoặc JSON serializer.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify bằng `./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** chỉ đọc [solution/README.md](solution/README.md) sau khi tự sửa.

## Expected Results
Trước: `reproduce` báo `REPRODUCED`; `verify` thất bại. Sau: `verify` báo `PASS` và hai frame được giữ nguyên; `reproduce` sẽ báo `NOT_REPRODUCED` vì lỗi đã biến mất.

## Estimated Time
40–50 phút.
