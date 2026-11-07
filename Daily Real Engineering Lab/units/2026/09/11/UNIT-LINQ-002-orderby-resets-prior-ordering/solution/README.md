# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms

Starter trả về đủ 3 invoice nhưng thứ tự là `INV-C,INV-B,INV-A`, khiến invoice `Priority=2` chen vào trước một invoice `Priority=1`.

## 2. Evidence

Dataset cho thấy `INV-A` và `INV-C` có `Priority=1`, còn `INV-B` có `Priority=2`. Vì vậy mọi kết quả đúng phải giữ cả hai invoice priority 1 trước `INV-B`, rồi mới xét `DueDate` bên trong nhóm priority 1.

## 3. Root cause

Lần gọi `OrderBy(x => x.DueDate)` thứ hai tạo một primary ordering mới và thay thế ý nghĩa ordering trước đó. Nó không bổ sung một secondary key cho `Priority`.

## 4. Why the fix works

`OrderBy(x => x.Priority).ThenBy(x => x.DueDate)` biểu diễn đúng contract: `Priority` là primary key, `DueDate` là secondary key trong các phần tử có cùng priority.

## 5. How to verify

Sau khi sửa `starter/Program.cs`, chạy:

```powershell
./verify.ps1
```

Kết quả phải chứa:

```text
COUNT=3
ORDER=INV-C,INV-A,INV-B
VERIFIED: ordering contract is satisfied.
```

## 6. Alternative fixes

Có thể tạo comparer riêng nếu ordering phức tạp, tái sử dụng nhiều nơi, hoặc chứa domain-specific rules. Với hai key đơn giản, `OrderBy` + `ThenBy` rõ ràng và ít abstraction hơn.

## 7. Wrong / Tempting Fixes

- Đảo thứ tự hai `OrderBy`: có thể tình cờ cho output đúng với dataset nhỏ nhưng vẫn không biểu diễn rõ primary/secondary ordering contract.
- Sort lại sau khi materialize bằng nhiều pass mutable: có thể đạt kết quả nhưng tăng cognitive load và dễ phụ thuộc vào stability assumptions.
- Hard-code thứ tự invoice IDs: chỉ che symptom, không sửa ordering logic.

## 8. Production implications

Semantic ordering bugs thường không throw exception và dễ lọt qua tests chỉ kiểm tra đủ phần tử. Với batch processing, ranking, scheduling hoặc queue prioritization, sai ordering có thể trở thành business correctness issue dù dữ liệu không mất.

## 9. Trade-offs

`ThenBy` tối ưu cho readability khi hierarchy của sort keys cố định. Nếu rules được cấu hình runtime hoặc số key lớn, dynamic ordering abstraction có thể phù hợp hơn nhưng cần tests bảo vệ thứ tự ưu tiên của từng key.

## 10. What a Senior engineer should notice

Không chỉ kiểm tra collection có đúng phần tử; phải kiểm tra các invariant mang nghĩa nghiệp vụ như ordering, grouping và tie-break rules. Khi đọc LINQ pipeline, phân biệt operator tạo ordering mới với operator mở rộng ordering hiện hữu.
