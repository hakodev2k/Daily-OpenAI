# UNIT-CHANNEL-001 — Cold-chain audit: telemetry accepted but missing

## Mục tiêu
Điều tra tính toàn vẹn của audit stream khi hệ thống nhận burst telemetry, phân biệt producer acknowledgment với bằng chứng đã được consumer xử lý.

## Bối cảnh thực tế
Kho thực phẩm cứu trợ nhận 8 mẫu nhiệt độ liên tiếp từ tủ lạnh. Service ghi nhận cả 8 lần tiếp nhận, nhưng báo cáo kiểm toán chỉ còn một phần mẫu. Không được đánh đổi mất dữ liệu kiểm toán để lấy throughput.

## Bạn cần làm gì
Chạy hệ thống, ghi lại producer acknowledgment và chuỗi audit thực tế; đưa ra ít nhất hai giả thuyết; sửa starter để bảo toàn đủ 8 mẫu và xử lý đúng khi producer/consumer có tốc độ khác nhau.

## Yêu cầu môi trường
.NET SDK 8+, PowerShell 7+ hoặc Windows PowerShell, không cần cloud/Docker.

## Chạy nhanh
Tại thư mục unit: `dotnet run --project starter/AuditLab.csproj` hoặc `./run.ps1`.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` (chạy starter nguyên bản, xác nhận triệu chứng); sau đó chạy `./verify.ps1` để thấy tiêu chí nghiệm thu chưa đạt. Không chỉnh solution để qua bài.

## Những gì cần quan sát
So sánh số lượng dòng `producer accepted=True`, số mẫu thực sự xuất hiện ở `audit count`, và chuỗi sequence. Quan sát lúc consumer bắt đầu đọc so với lúc producer ghi.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/Program.cs`.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ mở sau khi tự điều tra.

## Expected Results
Trước: cả 8 producer writes được chấp nhận nhưng audit thiếu sequence. Sau: audit giữ đủ 1–8 đúng thứ tự; verification exit code 0. Không dùng timing tuyệt đối làm điều kiện pass.

## Estimated Time
45 phút.
