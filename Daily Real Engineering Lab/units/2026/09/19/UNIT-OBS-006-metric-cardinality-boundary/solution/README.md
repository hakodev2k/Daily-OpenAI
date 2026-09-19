# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
120 request trên chỉ hai route tạo gần 120 metric series dù workload nghiệp vụ rất nhỏ.

## 2. Evidence
Series identity được tạo từ route, status class và một giá trị thay đổi cho từng request. Route và status có tập giá trị nhỏ; dimension còn lại tăng theo request volume.

## 3. Root cause
Một request-specific identifier có cardinality không bị chặn được dùng làm metric dimension. Metrics backend phải tạo series riêng cho mỗi tổ hợp dimension, nên số series tăng theo số request thay vì theo số nhóm vận hành cần aggregate.

## 4. Why the fix works
Giữ metric dimensions ở các giá trị bounded như route template và status class. Request identifier vẫn có thể được ghi vào logs hoặc traces để correlation mà không biến mỗi request thành một metric series riêng.

Ví dụ sửa method:

```csharp
static void RecordRequestMetric(HashSet<string> series, string route, string statusClass, string requestId)
{
    series.Add($"http.server.duration|route={route}|status={statusClass}");
    // requestId belongs in logs/traces when per-request correlation is required.
}
```

## 5. How to verify
Chạy `./verify.ps1`. Với hai route và hai status class, series count phải nằm dưới threshold 8 và output có `VERIFY_PASS`.

## 6. Alternative fixes
Nếu cần phân tích tenant, chỉ thêm tenant dimension khi tập tenant thực sự nhỏ, ổn định và chi phí đã được đánh giá. Với tenant count lớn, dùng logs/traces hoặc một dimension phân nhóm có bounded vocabulary.

## 7. Wrong or misleading fixes
Tăng telemetry quota chỉ trì hoãn tăng trưởng. Sampling request có thể giảm volume nhưng không sửa cardinality model nếu giá trị dimension vẫn gần như unique. Xóa toàn bộ dimensions làm metric rẻ hơn nhưng mất khả năng phân tích vận hành cần thiết.

## 8. Production implications
High-cardinality metrics làm tăng memory, ingestion/storage cost và query cost; một số backend còn drop series hoặc throttle telemetry khi vượt giới hạn.

## 9. Trade-offs
Dimension phong phú giúp slicing tốt hơn nhưng mỗi dimension nhân không gian tổ hợp. Thiết kế metric cần cân bằng khả năng aggregate, chi phí và boundedness của label values.

## 10. What a Senior engineer should notice
Metrics, logs và traces phục vụ các kiểu câu hỏi khác nhau. Identifier có giá trị cao cho correlation không đồng nghĩa nó phù hợp làm metric dimension. Senior engineer nên review telemetry schema như một data model có capacity constraints, không chỉ review code instrumentation.
