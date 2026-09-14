# Reference Solution

> Chỉ xem sau khi đã reproduce vấn đề và tự thử sửa.

## 1. Symptoms

Query hoàn thành bình thường nhưng danh sách customer đủ điều kiện rỗng thay vì `1,3,4`.

## 2. Evidence

`Customers` có `1,2,3,4`. Block list có `2` và một row legacy với `CustomerId = NULL`. Không có exception; sai lệch chỉ nằm ở result set.

## 3. Root cause

`NOT IN` phải chứng minh giá trị bên trái khác **mọi** giá trị từ subquery. Khi subquery chứa `NULL`, một phần phép so sánh trở thành `UNKNOWN`. Trong SQL three-valued logic, `WHERE` chỉ giữ row có predicate là `TRUE`, nên các customer tưởng như không bị block vẫn bị loại khỏi result.

## 4. Why the fix works

Biểu diễn yêu cầu nghiệp vụ dưới dạng anti-semi join:

```sql
SELECT c.Id
FROM Customers AS c
WHERE NOT EXISTS (
    SELECT 1
    FROM BlockedCustomers AS b
    WHERE b.CustomerId = c.Id
)
ORDER BY c.Id;
```

`NOT EXISTS` hỏi trực tiếp: có row block nào match customer hiện tại hay không. Row có `CustomerId = NULL` không match `c.Id`, nên không làm biến đổi truth value của các customer khác.

## 5. How to verify

Chép cách sửa tương đương vào `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Kết quả phải có:

```text
ACTUAL=1,3,4
RESULT=PASS
```

## 6. Alternative fixes

Có thể giữ `NOT IN` nếu subquery loại `NULL` một cách rõ ràng:

```sql
WHERE Id NOT IN (
    SELECT CustomerId
    FROM BlockedCustomers
    WHERE CustomerId IS NOT NULL
)
```

Cách này đúng với dataset hiện tại, nhưng `NOT EXISTS` thường diễn đạt requirement “không có row matching” trực tiếp hơn và ít phụ thuộc vào nullability contract của subquery.

## 7. Wrong / tempting fixes

- Xóa row `NULL` thủ công: chỉ xử lý dữ liệu hiện tại, không bảo vệ query trước dữ liệu tương tự trong tương lai.
- Đổi cột thành `NOT NULL` mà không phân tích domain: có thể là quyết định schema hợp lý, nhưng migration/data contract phải được đánh giá riêng; nó không thay thế việc hiểu semantics của query.
- Bắt exception hoặc retry query: vấn đề không phải transient failure và không có exception để retry giải quyết.
- Thêm index: có thể cải thiện performance nhưng không sửa correctness.

## 8. Production implications

Silent query correctness bugs nguy hiểm hơn exception ở chỗ pipeline có thể báo success trong khi bỏ sót dữ liệu. Với exclusion logic dùng cho billing, authorization, notifications hoặc compliance, cần test cả trường hợp `NULL`, empty set và duplicate rows.

## 9. Trade-offs

`NOT EXISTS` và `NOT IN` có thể được optimizer chuyển thành plan tương đương trong nhiều trường hợp, nhưng semantics khi có `NULL` khác nhau. Chọn biểu thức trước hết phải đúng với data contract; sau đó mới đánh giá execution plan và index.

## 10. What a Senior engineer should notice

- Phân biệt correctness issue với performance issue trước khi optimize.
- Không suy luận SQL predicate bằng boolean logic hai giá trị của C#.
- Nullability của schema là một phần của query contract.
- Regression test nên encode business invariant: chỉ customer có matching block row mới bị loại.
