# Reference Solution — chỉ xem sau khi đã tự reproduce và thử fix

## Symptoms
Bốn sự kiện nhập, ba sự kiện được chấp nhận thay vì hai. Dashboard có hai dòng nhìn như cùng một mã vé.

## Evidence
`E-001` chứa U+00E9 (é), trong khi `E-002` chứa U+0065 và U+0301 (e + combining acute). Cả hai hiển thị gần như nhau nhưng `StringComparer.Ordinal` trả false. `E-004` lặp y hệt `E-001` nên bị loại. `E-003` khác chữ hoa/chữ thường nên phải được giữ.

## Root cause
Định danh nghiệp vụ xem hai biểu diễn Unicode *canonically equivalent* là cùng một mã, nhưng khóa `HashSet` đang so sánh trực tiếp các UTF-16 code unit. Dữ liệu nhập từ hai thiết bị có thể dùng các biểu diễn Unicode khác nhau.

## Why the fix works
Tạo khóa so sánh `scan.PassCode.Normalize(NormalizationForm.FormC)` trước khi đưa vào `HashSet<string>(StringComparer.Ordinal)`. Unicode canonical normalization làm cho hai biểu diễn tương đương có cùng khóa. Giữ nguyên `BadgeScan` đầu tiên trong output để bảo toàn payload và audit. Code tham chiếu: [BadgeImporter.cs](BadgeImporter.cs).

## How to verify
Sau khi tự thử, có thể copy file tham chiếu vào `starter/BadgeImporter.cs`, rồi chạy `./verify.ps1`. Kết quả: exit code 0, Accepted IDs `E-001, E-003`. `./reproduce.ps1` lúc này **không còn** trả 0 vì triệu chứng gốc đã biến mất.

## Alternative fixes
Có thể chuẩn hóa một lần ở ingestion boundary rồi lưu thêm `CanonicalPassCode` để truy vấn nhất quán xuyên service và database. Nếu nghiệp vụ yêu cầu không phân biệt case, phải thiết kế thêm case-folding rõ ràng và test riêng; không được tự áp dụng cho mã vé đang phân biệt case.

## Wrong / tempting fixes
- `ToLowerInvariant()` đơn thuần không giải quyết canonical equivalence và còn gộp nhầm mã khác case.
- `StringComparer.CurrentCultureIgnoreCase` làm định danh phụ thuộc culture và thay đổi contract.
- Dùng `Trim()` hoặc loại ký tự dấu làm mất ý nghĩa mã và không giải quyết đúng cơ chế.
- Loại trùng theo EventId chỉ phát hiện sự kiện lặp, không phát hiện hai thiết bị quét cùng vé.

## Production implications
Quy tắc canonical identity phải nhất quán ở API, cache, database unique key, import batch và message consumer. Khi dữ liệu cũ đã lưu, cần backfill có kiểm soát, xác định collision policy và lưu raw input để audit. Canonical equivalence **không** đồng nghĩa với chống homoglyph/confusable hoặc Unicode security đầy đủ.

## Trade-offs
Chuẩn hóa tại chỗ đơn giản và đủ cho lab; chuẩn hóa ngay khi nhập giúp giảm logic trùng lặp nhưng có chi phí migration và versioning. FormC bảo toàn nhiều khác biệt hơn FormKC; không chọn compatibility normalization nếu chưa có yêu cầu nghiệp vụ.

## Senior engineer nên nhận ra
Phải viết hợp đồng identity trước khi chọn comparer; chứng minh bằng code points thay vì quan sát bằng mắt; giữ raw audit và canonical key riêng; kiểm tra ảnh hưởng xuyên storage/index; viết regression case cho composed/decomposed, case distinction, first-seen và idempotency.
