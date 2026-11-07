# Reference Solution — chỉ xem sau khi đã thử fix

## 1. Symptoms

`FindOneAndUpdateAsync` hoàn tất write, document trong MongoDB đã có `Status = Ready`, nhưng object được application nhận về vẫn là `Pending`.

## 2. Evidence

Starter in ra:

```text
BEFORE_STATUS=Pending
RETURNED_STATUS=Pending
STORED_STATUS=Ready
```

Điểm quan trọng là write state và returned state khác nhau dù chỉ có một operation.

## 3. Root cause

MongoDB .NET Driver mặc định dùng `ReturnDocument.Before` cho `FindOneAndUpdateAsync`. Operation update document atomically nhưng trả về phiên bản **trước update** nếu không cấu hình khác.

## 4. Why the fix works

Truyền `FindOneAndUpdateOptions<Shipment>` với `ReturnDocument = ReturnDocument.After`. Server vẫn thực hiện một find-and-update operation, nhưng document trả về là post-update state.

## 5. How to verify

Chép cách sửa tương đương vào `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Kỳ vọng:

```text
BEFORE_STATUS=Pending
RETURNED_STATUS=Ready
STORED_STATUS=Ready
```

## 6. Alternative fixes

Có thể update rồi gọi `Find` lần hai. Cách này đôi khi chấp nhận được nếu contract khác, nhưng thêm round-trip và read sau đó không còn là cùng một atomic return contract với update.

Nếu caller không cần document mới, một API chỉ trả write acknowledgement có thể rõ nghĩa hơn.

## 7. Wrong / tempting fixes

- Mutate object local thành `Ready` sau khi call: response có vẻ đúng nhưng che khuất contract thật và dễ sai khi server-side update phức tạp hơn.
- Update rồi `Task.Delay` trước khi đọc object cũ: timing không liên quan đến returned document semantics.
- Retry update: operation đầu đã thành công; retry có thể tạo side effect khác nếu update sau này không idempotent.
- Thêm cache để “ổn định” response: không giải quyết mismatch giữa write contract và returned document.

## 8. Production implications

Một stale response ngay sau write có thể làm UI rollback trạng thái, trigger logic sai ở caller, hoặc khiến client retry một operation đã thành công. Đây là contract bug chứ không nhất thiết là database consistency bug.

## 9. Trade-offs

`ReturnDocument.After` phù hợp khi caller cần post-update document. Nếu payload lớn và caller chỉ cần acknowledgement, không trả document có thể rẻ và rõ hơn. Hãy thiết kế API theo dữ liệu caller thực sự cần.

## 10. What a Senior engineer should notice

- Phân biệt persisted state với return semantics của driver/API.
- Đọc contract của operation thay vì suy diễn từ tên method.
- Tránh thêm round-trip chỉ để vá một default option chưa đúng với use case.
- Verification phải kiểm tra cả state lưu trữ và state trả cho caller.
