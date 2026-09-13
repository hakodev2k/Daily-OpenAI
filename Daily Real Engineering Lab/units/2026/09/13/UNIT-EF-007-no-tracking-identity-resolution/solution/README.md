# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Các `Order` có cùng `CustomerId`, nhưng mỗi order giữ một `Customer` CLR instance khác nhau. Functional values vẫn đúng nên test chỉ so field có thể không phát hiện vấn đề.

## 2. Evidence

Starter in:

- `Orders=3`
- `DistinctCustomerIds=1`
- `DistinctCustomerObjects>1`

Điều này chứng minh database identity và CLR object identity đang diverge trong read model.

## 3. Root cause

`AsNoTracking()` không sử dụng change tracker và cũng không thực hiện identity resolution theo cách tracking query làm. Khi cùng entity xuất hiện nhiều lần trong result graph, EF Core có thể materialize nhiều CLR instances cho cùng primary key.

## 4. Why the fix works

Dùng `AsNoTrackingWithIdentityResolution()` giữ query ở trạng thái read-only đối với `DbContext.ChangeTracker`, nhưng EF Core dùng một temporary identity map trong quá trình materialization để tái sử dụng cùng CLR instance cho cùng entity key.

## 5. How to verify

Chạy:

```powershell
./scripts/verify.ps1
```

Expected:

- `Orders=3`
- `DistinctCustomerIds=1`
- `DistinctCustomerObjects=1`
- exit code `0`

## 6. Alternative fixes

- Dùng tracking query nếu downstream thực sự cần unit-of-work semantics và sẽ update entities.
- Projection sang DTO/value model có thể tốt hơn nếu downstream không cần entity identity. Khi đó contract nên được thiết kế rõ ràng thay vì ngầm phụ thuộc `ReferenceEquals`.

## 7. Wrong or misleading fixes

- Tự deduplicate bằng dictionary sau khi query: có thể che symptom nhưng vẫn materialize dư objects và tăng complexity.
- Bật tracking toàn bộ chỉ để có identity resolution: có thể hợp lệ trong một số workflow, nhưng tạo tracking overhead và thay đổi semantics của read-only path.
- So sánh `CustomerId` thay vì object identity ở mọi nơi: đôi khi đúng về domain design, nhưng không giải quyết allocation/object graph nếu use case thực sự cần shared instance.

## 8. Production implications

Với result lớn có entity lặp lại nhiều lần, duplicate instances làm tăng allocation, GC pressure và có thể gây bug nếu code downstream dùng reference identity, in-memory cache hoặc graph transformation dựa trên object instance.

## 9. Trade-offs

`AsNoTrackingWithIdentityResolution()` thêm chi phí identity map tạm thời so với pure `AsNoTracking()`. Không phải mọi read query đều cần nó. Chọn behavior dựa trên shape của result và downstream contract.

## 10. What a Senior engineer should notice

`tracking` và `identity resolution` là hai concerns liên quan nhưng không đồng nhất. Tối ưu query không nên chỉ dừng ở câu hỏi “tracking hay no-tracking”; cần hiểu materialization semantics, result shape và downstream expectations.
