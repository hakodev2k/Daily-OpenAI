# UNIT-TCS-001 — Billboard campaign publish job bị kẹt

## Mục tiêu
Tái hiện một job bất đồng bộ không kết thúc, phân tích bằng chứng publisher/consumer, kiểm tra ít nhất ba hypothesis, tự sửa và xác nhận một chiến dịch chỉ được giao đúng một lần.

## Bối cảnh thực tế
Hệ thống bảng quảng cáo điện tử phát lịch campaign mới tới thiết bị. Job publish không hoàn thành dù đã phát tín hiệu; dashboard không ghi nhận exception. Retry thiếu kiểm soát có nguy cơ phát lịch trùng.

## Bạn cần làm gì
1. Ghi lại status, elapsedMs, publisherDone, subscriberDone và delivered.
2. Đối chiếu hypothesis: downstream chậm, exception bị nuốt, hoặc tương tác bất lợi trong vùng đồng bộ.
3. Chỉ chỉnh starter/Program.cs; giữ đúng một delivery và cả hai Task hoàn tất.
4. Chạy verify.ps1 trên code đã chỉnh, ghi bằng chứng và đánh giá production trade-offs.

## Yêu cầu môi trường
Windows PowerShell 5.1+ hoặc PowerShell 7; .NET 8 SDK/runtime. Không cần Docker, Azure, database hay NuGet ngoài.

## Chạy nhanh
Tại thư mục unit:

    .\setup.ps1
    .\run.ps1
    .\reproduce.ps1

## Cách reproduce vấn đề
Chạy reproduce.ps1 trên starter gốc trước khi sửa. Script phải in REPRODUCED, exit code 0. Sau khi sửa, reproduce không còn thành công là điều bình thường.

## Những gì cần quan sát
- status, elapsedMs, publisherDone, subscriberDone, delivered.
- Có exception hay chỉ không có tiến triển?
- Khi nào subscriber được đăng ký? Tín hiệu được hoàn tất trong điều kiện đồng bộ nào?
- Hoàn tất signal và hoàn tất toàn workflow có giống nhau?

## Quy tắc làm lab
1. Reproduce.
2. Ghi hypothesis và bằng chứng vào workspace/my-investigation.md.
3. Thử fix trong starter/.
4. Chạy verify.ps1 (kiểm tra starter của bạn, không phải reference).
5. Chỉ sau đó đọc solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
60 phút. L3 / D4. Validation mode: static-review; chưa chạy .NET khi tạo lab.
