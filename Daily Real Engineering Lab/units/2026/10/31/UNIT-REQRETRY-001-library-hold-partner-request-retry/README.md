# UNIT-REQRETRY-001 — Library Hold Relay: Partner 503 Retry Contract

## Mục tiêu
Điều tra một lỗi phát sinh trong quá trình chuyển yêu cầu giữ sách sang hệ thống thư viện đối tác; thu thập evidence và kiểm chứng retry policy mà không làm mất hợp đồng HTTP.

## Bối cảnh thực tế
Hệ thống liên thư viện gửi yêu cầu giữ sách đến partner. Partner thỉnh thoảng trả 503 trong đợt đồng bộ danh mục. Dashboard ghi nhận request đầu tiên thất bại, nhưng tác vụ chuyển tiếp không phục hồi như kỳ vọng, dù partner sẵn sàng nhận request kế tiếp.

## Bạn cần làm gì
- Chạy kịch bản lỗi trên một partner giả lập hoàn toàn local.
- Phân biệt phản hồi HTTP đầu tiên với exception ở tầng client.
- Đưa ra ít nhất hai hypothesis, sau đó sửa riêng starter/PartnerHoldClient.cs.
- Giữ nguyên method, path, JSON payload và Idempotency-Key qua các attempt.
- Kiểm chứng 503 tạm thời, 400 vĩnh viễn, 202 ngay lần đầu, 503 liên tiếp và cancellation.

## Yêu cầu môi trường
Windows PowerShell 5.1+ hoặc PowerShell 7, .NET SDK 8 trở lên; không cần Docker, Azure, NuGet ngoài SDK hay mạng ngoài.

## Chạy nhanh
Từ thư mục unit:

    pwsh -File ./scripts/reproduce.ps1

Nếu chỉ có Windows PowerShell:

    powershell -File ./scripts/reproduce.ps1

## Cách reproduce vấn đề
Chạy script reproduce khi starter còn nguyên. Kịch bản cố định: partner trả 503 rồi 202. Script phải ghi nhận failure đặc trưng và exit code 0 để xác nhận đã tái hiện thành công. Tiếp tục chạy:

    pwsh -File ./scripts/verify.ps1

Trên starter chưa sửa, verify phải thất bại (exit code khác 0). Sau khi sửa, verify phải thành công.

## Những gì cần quan sát
- Số request partner thực sự nhận được, không chỉ số attempt trong ứng dụng.
- Status của lần gửi đầu và exception cuối.
- Tính nhất quán của method, path, payload và Idempotency-Key.
- Có retry không cần thiết cho lỗi không tạm thời hoặc request đã bị hủy hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào workspace/my-investigation.md.
3. Thử fix chỉ trong starter/PartnerHoldClient.cs.
4. Verify trên chính starter đã sửa.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution — chỉ xem sau khi tự điều tra](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
55 phút. Độ khó D4; maturity L2.
