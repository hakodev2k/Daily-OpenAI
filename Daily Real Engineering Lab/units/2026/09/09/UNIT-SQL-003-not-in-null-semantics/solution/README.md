# Reference Solution — xem sau khi đã tự làm

## 1. Symptoms

Starter có ba customer active (`1,2,3`). Suppression chỉ chứa customer `2` cùng một row có `NULL`, nhưng query trả về tập rỗng thay vì `1,3`.

## 2. Evidence

Dữ liệu nguồn hợp lệ và deterministic. Failure xuất hiện ngay khi subquery của exclusion chứa `NULL`; không cần concurrency, network hay timing để reproduce.

## 3. Root cause

`NOT IN` tương đương về mặt logic với chuỗi comparison phủ định đối với mọi giá trị trong tập. Khi tập có `NULL`, comparison như `1 <> NULL` không phải `TRUE` mà là `UNKNOWN`. Trong SQL three-valued logic, `WHERE` chỉ giữ row khi predicate là `TRUE`, nên candidate có thể bị loại dù không có suppression row khớp nó.

## 4. Why the fix works

Reference solution dùng correlated `NOT EXISTS` để diễn đạt trực tiếp business invariant: chỉ giữ customer nếu **không tồn tại** suppression row có `CustomerId = c.Id`. Row suppression có `NULL` không match bất kỳ customer ID nào, nên không làm toàn bộ anti-filter trở thành `UNKNOWN`.

## 5. How to verify

Chạy từ root unit:

```powershell
./verify.ps1
```

Learner-editable `starter/` phải in:

```text
OBSERVED_ELIGIBLE_IDS=1,3
BUSINESS_RULE_MATCH=True
```

## 6. Alternative fixes

Một phương án khác là lọc `NULL` khỏi subquery trước khi dùng `NOT IN`. Cách đó có thể đúng nếu schema/business rule đảm bảo rõ ý nghĩa của `NULL`, nhưng nó gắn correctness vào việc luôn nhớ sanitize tập con. `NOT EXISTS` thường thể hiện anti-match intent trực tiếp hơn.

## 7. Wrong / Tempting Fixes

- Xóa row `NULL` khỏi seed data chỉ làm mất reproduction; dữ liệu production vẫn có thể tái xuất hiện.
- Thêm `OR CustomerId IS NULL` sai hướng vì row `NULL` không đại diện cho một customer cần exclude.
- Bọc giá trị bằng một sentinel tùy ý như `COALESCE(CustomerId, -1)` có thể chạy với dataset hiện tại nhưng tạo hidden assumption về domain của key.
- Đổi sang application-side filtering kéo thêm data qua boundary và không giải quyết mental model SQL.

## 8. Production implications

Bugs liên quan `NULL` thường xuất hiện sau import, schema evolution hoặc dirty historical data, nên có thể nằm im lâu dù query đã chạy production. Với exclusion logic, cần test dataset chứa: tập rỗng, một match, không match, `NULL`, và nhiều row duplicate.

## 9. Trade-offs

`NOT EXISTS` và anti-join có thể được optimizer xử lý tốt, nhưng performance vẫn cần đo trên engine/data thật. Lab này tập trung correctness trước optimization. Không đổi query chỉ vì một pattern được cho là “nhanh hơn”.

## 10. What a Senior engineer should notice

Senior engineer nên tách ba câu hỏi: business semantics là gì, SQL truth semantics thực tế là gì, và optimizer sẽ thực thi ra sao. Correctness phải được chứng minh trước rồi mới đánh giá execution plan/performance.
