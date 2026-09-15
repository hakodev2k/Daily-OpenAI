# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Read model đúng nhưng database/intermediate row work tăng rất nhanh khi hai sibling collections cùng lớn.

## 2. Evidence

Scenario có 1 order, 40 lines và 25 adjustments: chỉ 66 logical entities nhưng single relational shape có thể biểu diễn 1,000 row combinations.

## 3. Root cause

Khi nhiều collection navigation cùng cấp được JOIN trong một single query, mỗi row của collection này kết hợp với mỗi row của collection kia. Đây là cartesian explosion ở relational result shape, không phải số entity nghiệp vụ thực tế.

## 4. Why the fix works

Tách collection loading thành các query phù hợp làm work tăng gần theo tổng kích thước collections thay vì tích của chúng. Trong EF Core, `AsSplitQuery()` là một lựa chọn khi query shape phù hợp.

Trong mô phỏng starter, thay:

```csharp
var joinedRows = orderCount * linesPerOrder * adjustmentsPerOrder;
```

bằng mô hình split work:

```csharp
var joinedRows = orderCount + linesPerOrder + adjustmentsPerOrder;
```

Trong application thật, quyết định nằm ở EF query, ví dụ đánh giá `AsSplitQuery()` thay vì chỉ thay phép tính mô phỏng.

## 5. How to verify

Chạy `./verify.ps1`. Script kiểm tra cả business result và giới hạn intermediate work trên learner-editable `starter/`.

## 6. Alternative fixes

- Projection chỉ lấy fields cần thiết.
- Tách query thủ công khi cần kiểm soát rõ transaction/consistency.
- Giới hạn hoặc paginate collection nếu contract cho phép.

## 7. Wrong or misleading fixes

- Chỉ tăng DB resources: có thể che triệu chứng nhưng không sửa query shape.
- Thêm `AsNoTracking()` và kỳ vọng hết amplification: tracking overhead khác với số row do JOIN tạo ra.
- Luôn bật split query toàn cục: có trade-off về round trips và consistency, không phải mặc định tốt cho mọi query.

## 8. Production implications

Cartesian amplification làm tăng network payload, DB work, materialization, memory và latency; ảnh hưởng tăng mạnh theo cardinality thực tế.

## 9. Trade-offs

Split query giảm cartesian explosion nhưng tạo nhiều round trips và có thể quan sát dữ liệu thay đổi giữa các query nếu không có consistency boundary phù hợp. Projection thường hiệu quả hơn nếu endpoint không cần full graph.

## 10. What a Senior engineer should notice

Đừng tối ưu EF chỉ bằng số entity trả về. Hãy xem SQL/query shape, cardinality, row count, payload, materialization và contract của endpoint; chọn projection, single query hay split query dựa trên evidence và consistency requirement.
