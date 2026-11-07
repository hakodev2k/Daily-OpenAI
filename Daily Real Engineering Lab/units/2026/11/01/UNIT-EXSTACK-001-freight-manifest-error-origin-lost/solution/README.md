# Reference Solution — SPOILER

Chỉ xem sau khi reproduce và tự thử fix.

## 1. Symptoms
Dòng manifest lỗi vẫn bị reject và audit có một event, nhưng exception telemetry chỉ còn tầng điều phối, thiếu frame của parser.

## 2. Evidence
`reproduce.ps1` chạy thành công ở starter vì frame `SourceParser.Parse` vắng mặt. `verify.ps1` thất bại trước sửa. Exception vẫn là `FormatException`, không phải lỗi bị swallow.

## 3. Root cause
Trong `ImportCoordinator.Import`, nhánh `catch` ghi audit rồi `throw error;`. Câu lệnh này ném lại object exception từ một call site mới và làm mất phần stack gốc đã được ghi nhận ở tầng parser.

## 4. Why the fix works
Dùng `throw;` ngay trong `catch` để rethrow exception hiện tại, bảo toàn stack trace đã có. Giữ nguyên filter, audit và exception type. File mẫu: [ImportCoordinator.cs](ImportCoordinator.cs).

## 5. How to verify
Từ thư mục unit, sao chép nội dung file solution vào `starter/ImportCoordinator.cs` **sau khi đã tự thử**. Chạy `./scripts/verify.ps1`: phải exit 0; `SourceParser.Parse` và `ImportCoordinator.Import` cùng xuất hiện. `./scripts/reproduce.ps1` sau sửa phải exit khác 0 vì không còn symptom. Cả hai script kiểm tra `starter/`, không chạy file solution riêng.

## 6. Alternative fixes
Nếu cần giữ exception để rethrow sau khi ra khỏi `catch` hoặc qua async boundary, dùng `System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw()`. Trong cùng `catch`, `throw;` là giải pháp đơn giản nhất. Bọc bằng exception mới có thể phù hợp khi cần thêm domain context, nhưng phải giữ `InnerException` và định nghĩa contract rõ ràng.

## 7. Wrong or misleading fixes
- Thay `throw error;` bằng `throw new Exception(error.Message);`: mất type, stack và context.
- Chỉ thêm log message: có thể giúp tìm kiếm nhưng không phục hồi provenance của exception đã truyền đi.
- Bỏ `catch`: có thể phục hồi stack nhưng làm mất audit requirement; không đáp ứng toàn bộ contract.
- Dựa vào số dòng stack trace tuyệt đối: không ổn định giữa build, OS và runtime.

## 8. Production implications
Telemetry/incident triage phải giữ nguyên nguyên nhân ban đầu; audit log không được biến thành nơi che mất exception provenance. Có thể bật rule CA2200 trong CI để ngăn lỗi tái xuất hiện.

## 9. Trade-offs
Giữ stack bằng `throw;` không bổ sung business context mới. Nếu hệ thống cần thêm correlation ID hoặc manifest ID, dùng structured logging hoặc exception wrapping có `InnerException` với contract đã thỏa thuận, tránh ghi dữ liệu nhạy cảm.

## 10. Senior engineer should notice
Phải kiểm tra cả đường thành công, đường lỗi, exception type, audit side effect và stack provenance. “Không crash” hoặc “log đã ghi” không chứng minh observability còn đúng. Regression gate phải thất bại với starter và thành công với code học viên sửa.

## References
- Microsoft Learn: [Exception handling statements](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/exception-handling-statements)
- Microsoft Learn: [CA2200: Rethrow to preserve stack details](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca2200)
- Microsoft Learn: [ExceptionDispatchInfo](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.exceptionservices.exceptiondispatchinfo)
