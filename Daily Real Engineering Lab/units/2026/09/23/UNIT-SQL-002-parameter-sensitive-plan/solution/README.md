# Reference Solution — chỉ xem sau khi đã reproduce và thử fix

## 1. Symptoms
Cùng procedure trả đúng dữ liệu nhưng performance phụ thuộc mạnh vào tenant và đôi khi vào request nào compile trước sau khi cache cold.

## 2. Evidence
Dataset cố ý có cardinality skew lớn. Khi một execution plan được compile cho một parameter shape rồi reuse cho shape rất khác, Estimated-vs-Actual rows và operator economics có thể lệch mạnh. Logical reads và elapsed time cho thấy regression rõ hơn timing đơn lẻ.

## 3. Root cause
Đây là parameter-sensitive plan problem: một cached plan được tối ưu dựa trên cardinality/parameter context của lần compile nhưng được reuse cho workload có distribution khác đáng kể. Vấn đề không phải đơn giản là “thiếu index”; index hiện tại vẫn có thể hợp lý cho một shape và kém tối ưu cho shape khác.

## 4. Why the fix works
Không có một fix duy nhất cho mọi workload. Mục tiêu là tránh ép các cardinality shape rất khác nhau dùng một execution strategy không phù hợp.

Với SQL Server 2022 và compatibility level phù hợp, Parameter Sensitive Plan optimization có thể giữ nhiều plan variants cho cùng parameterized statement. Với hệ thống/version khác, statement-level `OPTION (RECOMPILE)` có thể phù hợp khi execution không quá thường xuyên và compile overhead chấp nhận được. `OPTIMIZE FOR` hoặc tách query shape chỉ nên dùng khi workload distribution đủ ổn định và bạn hiểu trade-off.

Một thử nghiệm đơn giản cho lab là tạo variant query trực tiếp với `OPTION (RECOMPILE)` rồi so sánh cả hai tenant. Đây là reference experiment, không phải mặc định production recommendation.

```sql
SELECT Id,TenantId,CreatedAt,Status,Amount
FROM dbo.Orders
WHERE TenantId=@TenantId AND CreatedAt>=@From
ORDER BY CreatedAt DESC
OPTION (RECOMPILE);
```

## 5. How to verify
- Giữ nguyên row set.
- Chạy cả tenant nhỏ và lớn nhiều lần.
- So sánh logical reads, Actual-vs-Estimated rows và operator choices.
- Kiểm tra behavior sau cold cache và warm cache.
- Nếu chọn recompilation, đo compile CPU/cost ở workload đại diện.

## 6. Alternative fixes
- SQL Server 2022 Parameter Sensitive Plan optimization khi workload và compatibility level đáp ứng điều kiện.
- `OPTION (RECOMPILE)` cho statement phù hợp.
- `OPTIMIZE FOR` khi có một representative value thực sự ổn định.
- Tách query/procedure path theo cardinality class khi business distribution rõ và maintenance cost chấp nhận được.
- Query Store hints trong môi trường phù hợp, sau khi có evidence.

## 7. Wrong / tempting fixes
- Thêm index mới ngay lập tức: có thể tăng write/storage cost nhưng không giải quyết plan reuse mismatch.
- `DBCC FREEPROCCACHE` định kỳ: chỉ reset symptom và gây compile churn toàn hệ thống.
- Ép một plan duy nhất từ tenant lớn hoặc tenant nhỏ mà không đo distribution: chỉ chuyển regression sang nhóm còn lại.
- Scale database trước khi hiểu plan behavior: tăng chi phí nhưng không sửa cơ chế.

## 8. Production implications
Theo dõi Query Store, plan changes, runtime stats theo query/plan và workload segmentation. Data distribution thay đổi theo thời gian nên một mitigation từng tốt có thể trở nên kém phù hợp.

## 9. Trade-offs
Plan specialization cải thiện runtime cho skewed workloads nhưng đổi lại compile overhead, nhiều plan variants hoặc maintenance complexity. Senior engineer phải chọn theo frequency, skew, latency SLO, CPU headroom và khả năng vận hành.

## 10. What a Senior engineer should notice
Không kết luận “parameter sniffing” chỉ vì query chậm thất thường. Phải chứng minh mối liên hệ giữa parameter cardinality, compiled plan, plan reuse và runtime evidence. Sau đó mới chọn mitigation có phạm vi nhỏ nhất và đo regression cho các workload shape khác.