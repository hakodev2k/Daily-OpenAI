# Reference Solution — chỉ xem sau khi đã reproduce và tự thử fix

## Symptoms
Audit middleware thấy đầy đủ payload nhưng endpoint từ chối webhook hợp lệ; endpoint đọc được body rỗng.

## Evidence
Cùng request có audit byte count > 0 trong khi endpoint báo body length = 0. Signature được tạo từ payload gốc nên không khớp HMAC của chuỗi rỗng.

## Root cause
Middleware đọc `HttpRequest.Body` trước endpoint. Request body là stream và lần đọc đầu làm vị trí đọc đi đến cuối. Starter không tạo khả năng đọc lại và không đưa vị trí stream về đầu trước khi chuyển pipeline.

## Why the fix works
`EnableBuffering()` cho phép request body được đọc lại. Sau audit read, đặt `Body.Position = 0` để downstream consumer nhận cùng payload.

## How to verify
Chạy starter đã sửa bằng `run.ps1`, sau đó `verify.ps1`. Request phải trả accepted=true và console audit vẫn ghi byte count > 0.

## Alternative fixes
Thiết kế pipeline để chỉ một component đọc raw body rồi truyền representation đã xác thực xuống downstream có thể phù hợp khi payload lớn hoặc cần ownership rõ hơn.

## Wrong / tempting fixes
- Bỏ signature validation: che symptom và phá security contract.
- Hard-code signature/payload: chỉ làm test pass giả tạo.
- Đọc body ở endpoint trước middleware: middleware ordering không giải quyết ownership tổng quát nếu nhiều component vẫn cần raw body.

## Production implications
Buffering có chi phí memory/disk tùy kích thước và threshold. Với webhook lớn cần giới hạn request size, tránh log dữ liệu nhạy cảm, và xác định rõ component nào được phép đọc raw body.

## Trade-offs
Buffer-and-rewind đơn giản cho payload nhỏ. Centralized raw-body capture có ownership rõ hơn nhưng tăng abstraction và coupling.

## Senior engineer should notice
Request body là resource có lifecycle/ownership trong pipeline. Cross-cutting middleware phải bảo toàn downstream contract thay vì giả định việc quan sát request là side-effect-free.