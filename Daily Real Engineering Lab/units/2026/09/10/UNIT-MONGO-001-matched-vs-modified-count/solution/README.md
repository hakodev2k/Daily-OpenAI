# Reference Solution

> Chỉ xem sau khi đã reproduce vấn đề và thử fix trong `starter/`.

## 1. Symptoms

Request đầu tiên thay đổi `DisplayName` thành công. Request retry với cùng payload lại bị phân loại là `NOT_FOUND` mặc dù document vẫn tồn tại.

## 2. Evidence

MongoDB trả hai signal khác nhau:

- `MatchedCount` cho biết filter đã match bao nhiêu document.
- `ModifiedCount` cho biết bao nhiêu document thực sự bị thay đổi dữ liệu.

Một idempotent retry có thể có `MatchedCount = 1` và `ModifiedCount = 0`.

## 3. Root cause

Starter dùng `ModifiedCount == 0` làm bằng chứng rằng document không tồn tại. Hai khái niệm “không match document” và “match nhưng giá trị mới giống giá trị hiện tại” bị gộp thành một.

## 4. Why the fix works

Dùng `MatchedCount == 0` để quyết định `NOT_FOUND`. Nếu document match nhưng update là no-op thì operation vẫn hợp lệ và trả `OK`.

## 5. How to verify

Chạy:

```powershell
./verify.ps1
```

Kỳ vọng:

- update thay đổi dữ liệu → `OK`
- retry cùng payload → `OK`
- id không tồn tại → `NOT_FOUND`

## 6. Alternative fixes

Nếu API contract cần phân biệt “updated” và “already current”, có thể dùng cả `MatchedCount` và `ModifiedCount` để trả internal outcome khác nhau, nhưng không nên biến no-op trên document tồn tại thành `404`.

Một lựa chọn khác là đọc document trước rồi quyết định, nhưng thường thêm round trip và mở thêm race window mà không cần thiết.

## 7. Wrong / tempting fixes

- Retry lại khi `ModifiedCount == 0`: không giải quyết semantics; cùng payload sẽ tiếp tục là no-op.
- Thêm `Find` trước `UpdateOne`: có thể che symptom nhưng tăng I/O và không bảo đảm state không đổi giữa hai operation.
- Ép update thêm trường timestamp chỉ để `ModifiedCount` luôn > 0: làm thay đổi dữ liệu nhằm thỏa một signal sai, đồng thời tạo write amplification.

## 8. Production implications

Nhầm lẫn giữa “matched” và “modified” có thể tạo false 404, retry storm, alert sai và telemetry sai về missing records. Lỗi này đặc biệt dễ xuất hiện ở idempotent PUT/PATCH-like workflows.

## 9. Trade-offs

`MatchedCount` phù hợp cho existence semantics của chính update filter. `ModifiedCount` vẫn hữu ích cho audit, metrics hoặc business logic cần biết dữ liệu có thật sự đổi hay không.

## 10. What a Senior engineer should notice

Senior engineer nên tách rõ contract của storage driver khỏi HTTP/business semantics: mỗi field trong driver result trả lời một câu hỏi khác nhau. Trước khi mapping storage outcome sang status code, cần xác định chính xác câu hỏi mà API đang muốn trả lời.
