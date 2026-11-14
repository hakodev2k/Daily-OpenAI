# UNIT-CHANREAD-001 — Hai worker nhận kết quả xét nghiệm

## Mục tiêu
Điều tra một lỗi concurrency trong hệ thống worker .NET, phân biệt tín hiệu có dữ liệu với quyền sở hữu một phần tử, và chứng minh tính đúng đắn qua replay có kiểm soát.

## Bối cảnh thực tế
Phòng khám cộng đồng nhận kết quả xét nghiệm từ thiết bị và có hai worker xử lý song song. Trong một đợt tải thấp, chỉ có một kết quả mới nhưng đôi khi một worker ghi nhận lỗi nghiêm trọng dù kết quả đã được xử lý thành công.

## Bạn cần làm gì
Chạy starter, ghi lại trạng thái từng worker và số kết quả được xử lý; nêu ít nhất hai giả thuyết, sửa duy nhất `starter/Program.cs` rồi chạy verify. Không thay đổi contract: một kết quả chỉ được xử lý một lần, worker còn lại kết thúc bình thường khi nguồn đóng.

## Yêu cầu môi trường
.NET SDK 8.0.x, PowerShell 7+ hoặc Windows PowerShell 5.1; không cần Docker, database hay NuGet ngoài SDK.

## Chạy nhanh
Từ thư mục unit: `./scripts/reproduce.ps1`, sau đó `./scripts/verify.ps1` sau khi tự sửa starter.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1`. Replay đồng bộ hóa hai worker ở cùng một thời điểm quan sát; ghi lại `OBSERVED` và `RESULT`. Có thể chạy lại nhiều lần, kết quả mong đợi vẫn nhất quán.

## Những gì cần quan sát
Đếm số worker báo lỗi, số lần xử lý mã kết quả và trạng thái kết thúc. So sánh số bản ghi đầu vào với số kết quả đã xử lý. Kiểm tra thứ tự giữa bước kiểm tra khả dụng và bước lấy dữ liệu.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Thử fix trong starter.
4. Verify bằng script riêng.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md)

## Reference Solution
**Spoiler:** [Lời giải và trade-offs](solution/README.md) — chỉ đọc sau khi tự thử.

## Expected Results
Starter: đúng một kết quả được xử lý nhưng một worker có thể báo lỗi; script reproduce coi đây là triệu chứng thành công. Sau fix: đúng một kết quả được xử lý, không có worker báo lỗi, cả hai kết thúc bình thường; script verify trả exit code 0.

## Estimated Time
45–60 phút.
