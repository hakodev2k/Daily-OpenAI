# UNIT-UTF8CHUNK-001 — Nhật ký sửa xe đạp: chữ tiếng Việt hỏng khi truyền theo từng đoạn

## Mục tiêu
Điều tra sự khác biệt giữa dữ liệu văn bản gửi đi và bản ghi được lưu sau khi truyền thành nhiều đoạn; thu thập evidence ở byte boundary, đưa ra giả thuyết, sửa bộ xử lý và kiểm chứng cả dữ liệu hợp lệ lẫn payload lỗi.

## Bối cảnh thực tế
Một hệ thống bike-share tiếp nhận ghi chú bảo trì từ kiosk. Bản ghi `Đà Nẵng: xe đạp cần sửa phanh.` đúng khi gửi qua một đoạn nhưng đôi khi xuất hiện ký tự thay thế trong báo cáo sau khi gateway chuyển tiếp thành nhiều đoạn. Bản ghi chỉ có ASCII vẫn đúng. Đội vận hành không được phép sửa dữ liệu gốc hay bỏ dấu tiếng Việt.

## Bạn cần làm gì
1. Chạy starter và ghi lại trạng thái của các case `one-byte`, `mixed`, `ascii` và payload lỗi.
2. So sánh byte count, số chunk và số ký tự thay thế. Viết ít nhất hai hypothesis trước khi đọc hints.
3. Chỉ chỉnh sửa `starter/ChunkedNoteDecoder.cs`; giữ nguyên `starter/Program.cs` và các điều kiện kiểm thử.
4. Chứng minh dữ liệu UTF-8 hợp lệ được khôi phục nguyên văn dù chunking thay đổi; payload không hợp lệ phải bị từ chối.
5. Phân tích ranh giới trách nhiệm giữa transport, decoding và validation.

## Yêu cầu môi trường
.NET 8 SDK, PowerShell 5.1+ hoặc 7; không cần NuGet ngoài, Docker, tài khoản cloud hay dữ liệu thật.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Trên starter nguyên bản, `./reproduce.ps1` trả exit 0 khi triệu chứng **được tái hiện**. `./verify.ps1` trả lỗi trước khi sửa và phải thành công trên **starter đã được bạn sửa**.

## Những gì cần quan sát
Tập trung vào `correct`, `rejected`, `replacementChars` và `chunks` trong console. Ghi nhận sự khác biệt giữa một thông điệp logic và các cách chia đoạn truyền tải; không kết luận từ một case ASCII duy nhất.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và evidence vào `workspace/my-investigation.md`.
3. Thử fix trong starter.
4. Verify cả happy path và malformed input.
5. Chỉ sau đó mới xem reference solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Phân tích và trade-offs](solution/README.md) · [Reference code](solution/ChunkedNoteDecoder.cs). Chỉ xem sau khi tự điều tra.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
50 phút · L2 Investigation / D4.
