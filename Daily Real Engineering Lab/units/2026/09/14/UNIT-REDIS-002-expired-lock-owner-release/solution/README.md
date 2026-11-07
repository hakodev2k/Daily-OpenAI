# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Worker A acquire lock, lease của A hết hạn, B acquire lock mới, sau đó cleanup muộn của A xóa key hiện tại. C acquire được trong khi B vẫn đang ở critical section.

## 2. Evidence

- A và B không sở hữu cùng một lease.
- Khi A cleanup, owner hiện tại là B.
- Sau cleanup của A, key biến mất dù B chưa release.
- C acquire thành công và tạo overlap.

## 3. Root cause

Release chỉ dựa trên lock key, không xác minh ownership token. Một stale holder có thể xóa lease mới hơn của worker khác sau khi TTL cũ đã hết và lock được reacquire.

## 4. Why the fix works

Mỗi acquire gắn một token duy nhất vào lock value. Release chỉ được phép xóa khi value hiện tại vẫn bằng token của caller. Trong Redis thật, compare-and-delete phải atomic, thường thực hiện bằng Lua script hoặc primitive tương đương.

## 5. How to verify

Sửa `Release` trong starter để gọi ownership-aware release, rồi chạy:

```powershell
./verify.ps1
```

Expected: `SAFE`, exit code 0.

## 6. Alternative fixes

- Dùng thư viện distributed lock đã implement ownership token và atomic release đúng semantics.
- Với operation dài hơn TTL, cân nhắc lease renewal có kiểm soát, nhưng renewal cũng phải xác minh ownership.
- Nếu business operation có thể được thiết kế idempotent/serialized bằng database constraint hoặc queue partitioning, có thể tránh distributed lock hoàn toàn.

## 7. Wrong / Tempting Fixes

- **Tăng TTL thật lớn:** giảm xác suất nhưng không loại race khi operation vẫn có thể vượt TTL.
- **DEL trong finally vô điều kiện:** cleanup tốt về mặt cấu trúc nhưng vẫn xóa lock của owner mới.
- **Retry acquire nhanh hơn:** tăng contention và không sửa ownership semantics.
- **Dùng process-local lock:** không bảo vệ nhiều instance.

## 8. Production implications

Race này thường hiếm và phụ thuộc timing, nên dễ bị bỏ sót trong test thông thường. Hậu quả có thể là duplicate processing, double side effects hoặc concurrent mutation trên cùng aggregate.

## 9. Trade-offs

Distributed lock thêm failure modes: TTL sizing, clock/latency assumptions, renewal, network partition và fencing. Khi consistency requirement cao, ownership token đơn thuần có thể chưa đủ; fencing token hoặc transactional coordination có thể phù hợp hơn.

## 10. What a Senior engineer should notice

Senior engineer phân biệt **lock key tồn tại** với **caller còn quyền sở hữu lease hiện tại**. Release là một state transition cần identity/ownership proof, không chỉ là cleanup theo key.
