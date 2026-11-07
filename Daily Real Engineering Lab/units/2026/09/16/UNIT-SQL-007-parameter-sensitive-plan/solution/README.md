# Reference Solution — chỉ xem sau khi đã tự điều tra

## 1. Symptoms
Cùng procedure có latency và logical reads khác biệt lớn; behavior thay đổi theo execution đầu tiên sau compile/restart.

## 2. Evidence
Data distribution lệch mạnh giữa Tenant A và B. Plan được compile/reuse trong mỗi giai đoạn cho workload có cardinality khác với parameter ban đầu. Estimate-vs-actual mismatch lớn là signal quan trọng; app CPU và network RTT gần như ổn định là evidence chống lại các hypothesis application/network.

## 3. Root cause
Workload là parameter-sensitive. Một cached execution plan phù hợp với nhóm cardinality nhỏ có thể rất tệ cho tenant cực lớn, và ngược lại. Đây là vấn đề plan selection/reuse trên data distribution skewed, thường được gọi trong thực tế là parameter sniffing problem.

## 4. Why the fix works
Không có một mitigation duy nhất. Mục tiêu là cho optimizer có strategy phù hợp với các cardinality classes thay vì bắt mọi parameter dùng một plan không đại diện.

Trên SQL Server/compatibility level hỗ trợ Parameter Sensitive Plan (PSP), hãy đánh giá PSP trước. Với hệ thống cũ hoặc query không đủ điều kiện, các lựa chọn có thể gồm query branching theo business/cardinality class, targeted `OPTION (RECOMPILE)`, hoặc các query shapes riêng. Chọn dựa trên execution frequency, compile cost và distribution stability.

## 5. How to verify
- Capture actual execution plans cho small và large tenant.
- So sánh estimated vs actual rows, operators, logical reads, duration.
- Kiểm tra Query Store để xác nhận plan history/regression.
- Sau mitigation, test cả hai cardinality classes và workload mix; không chỉ test tenant gây incident.

## 6. Alternative fixes
- PSP khi platform và query đủ điều kiện.
- Branch/query shape riêng cho outlier tenants.
- `OPTION (RECOMPILE)` cho query tần suất thấp khi compile overhead chấp nhận được.
- Query Store plan forcing chỉ khi một plan thực sự đủ tốt cho workload mix và đã hiểu trade-off.

## 7. Wrong / Tempting Fixes
- Thêm index ngay mà chưa chứng minh bottleneck: có thể không giải quyết plan sensitivity.
- Force plan lấy từ Tenant B cho mọi tenant: có thể chuyển regression sang nhóm nhỏ.
- `OPTION (RECOMPILE)` cho mọi query: có thể tăng CPU compile đáng kể ở workload tần suất cao.
- Restart service/SQL để “fix”: chỉ thay execution history, không giải quyết cơ chế.

## 8. Production implications
Incident có thể trông ngẫu nhiên vì restart, deploy hoặc recompile thay đổi parameter dùng lúc compile. Monitoring chỉ theo average latency dễ che mất outlier tenant và plan regression.

## 9. Trade-offs
Mitigation tốt cân bằng runtime efficiency, compile CPU, plan cache behavior, maintainability và khả năng data distribution thay đổi theo thời gian.

## 10. What a Senior engineer should notice
Signal quan trọng nhất không phải query chậm đơn thuần mà là **performance phụ thuộc vào compile/execution history trên distribution skewed**. Senior engineer cần chứng minh điều này bằng plan/evidence trước khi chọn hint hoặc index.