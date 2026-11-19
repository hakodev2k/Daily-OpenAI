# UNIT-METRICLABEL-001 — Telemetry phòng khám tăng đột biến số lượng time series

## Mục tiêu
Điều tra sự tăng trưởng metric series không tương xứng với lưu lượng; phân biệt số request và số chuỗi đo; sửa instrumentation mà không làm mất phân loại HTTP status.

## Bối cảnh thực tế
Một hệ thống đặt lịch phòng khám nhận vài trăm request mỗi phút. Số lượng metric series tăng liên tục, dashboard chậm và chi phí lưu trữ observability tăng. Số request và các nhóm HTTP status vẫn có vẻ bình thường.

## Bạn cần làm gì
Chạy workload mô phỏng, ghi lại evidence, đưa ra ít nhất hai giả thuyết, sửa code trong `starter/Program.cs`, sau đó chứng minh telemetry có giới hạn và vẫn phản ánh đủ request.

## Yêu cầu môi trường
.NET SDK 8; PowerShell 7 hoặc Windows PowerShell; không cần Docker, cloud hay NuGet bên ngoài.

## Chạy nhanh
Tại thư mục unit: `./run.ps1`.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` trên starter nguyên bản. Script trả exit code 0 **khi triệu chứng được tái hiện**, không có nghĩa hệ thống đã đúng.

## Những gì cần quan sát
Ghi `requests`, `series`, `statuses` và tỷ lệ series/request. So sánh với ngân sách tối đa **4 series** cho hai route và hai HTTP status trong workload cố định.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis tại [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1` (exit 0 = đạt yêu cầu).
5. Chỉ sau đó xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution](solution/README.md). Chỉ mở sau khi tự thử.

## Expected Results
Trước: 240 request, nhiều hơn 4 series, reproduce PASS nhưng verify FAIL. Sau: vẫn 240 request, không quá 4 series, status 200/503 còn nguyên, verify PASS.

## Estimated Time
40–50 phút.
