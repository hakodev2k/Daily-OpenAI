# UNIT-HTTP-012 — Đợt retry chuyển sách thất bại trước khi gửi lần hai

## Mục tiêu
Điều tra một incident HTTP outbound có retry, xác định nguyên nhân từ bằng chứng của client và đối tác, triển khai sửa chữa có regression checks mà không thay đổi hợp đồng idempotency.

## Bối cảnh thực tế
Dịch vụ điều phối chuyển sách giữa các chi nhánh thư viện gọi một partner API. Khi partner phản hồi HTTP 503 ở lần đầu, worker đáng lẽ thử lại một lần. Sau bản cập nhật, job dừng với exception phía client, trong khi partner chỉ ghi nhận một request. Các lệnh không gặp 503 vẫn chạy bình thường.

## Bạn cần làm gì
1. Chạy harness và ghi status, số lần partner nhận request, exception và payload.
2. Nêu ít nhất hai hypothesis; phân biệt lỗi partner, lỗi serialization và lỗi client lifecycle bằng evidence.
3. Chỉ sửa starter/TransferDispatcher.cs; giữ nguyên harness và hợp đồng TransferCommand.
4. Verify cả 503→202, 400 không retry, 503→503 và payload/idempotency consistency.
5. Viết ngắn trade-off giữa retry trong service và resilience handler tập trung.

## Yêu cầu môi trường
.NET SDK 8.0+ và PowerShell 5.1+/7+. Không cần Docker, Azure, internet hoặc NuGet package ngoài SDK.

## Chạy nhanh
Từ thư mục unit:

~~~powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
~~~

Sau khi tự sửa starter:

~~~powershell
./scripts/verify.ps1
~~~

Không có PowerShell:

~~~sh
dotnet run --project starter/TransferLab.csproj -c Release -- --mode reproduce
dotnet run --project starter/TransferLab.csproj -c Release -- --mode verify
~~~

## Cách reproduce vấn đề
reproduce.ps1 thành công (exit 0) chỉ khi triệu chứng gốc xuất hiện. verify.ps1 kiểm tra starter do bạn sửa và trả exit khác 0 nếu hợp đồng chưa đạt. Hai bước không thay thế cho nhau.

## Những gì cần quan sát
- Partner nhận bao nhiêu request khi status đầu tiên là 503?
- Lỗi xảy ra ở bước nào của chu kỳ retry?
- Với request đầu tiên, body và Idempotency-Key có đúng không?
- Khi partner trả 400 ngay lần đầu, hệ thống có dừng đúng không?
- Với hai lần 503, số lần thử có bị vượt giới hạn không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ xem sau khi tự điều tra và verify.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). Ghi chép vào [workspace](workspace/my-investigation.md).

## Estimated Time
60 phút. Nếu cần định hướng điều tra, xem [investigation guide](docs/investigation-guide.md).
