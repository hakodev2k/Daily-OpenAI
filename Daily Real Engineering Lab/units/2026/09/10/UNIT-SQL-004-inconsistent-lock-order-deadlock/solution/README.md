# Reference Solution

## Symptoms

Các operation chạy riêng lẻ đều đúng, nhưng khi hai operation đụng cùng một cặp location theo thứ tự ngược nhau, mỗi bên có thể giữ một resource và chờ resource còn lại.

## Evidence

Timeline cho thấy A giữ Location 1 rồi chờ Location 2, trong khi B giữ Location 2 rồi chờ Location 1. CPU, storage latency và connection pool đều bình thường, nên đây không phải capacity incident.

## Root cause

Các code path không có một resource acquisition order thống nhất. Cùng một tập resource được xử lý theo hướng business request thay vì theo một canonical order, tạo điều kiện cho circular wait.

## Một defensible design

Chuẩn hóa thứ tự resource trước khi thực hiện thay đổi. Ví dụ, luôn xử lý location có `LocationId` nhỏ hơn trước, bất kể direction của business operation. Transaction boundary vẫn bao trọn cả hai thay đổi để giữ invariant.

## Vì sao cách này hoạt động

Nếu mọi operation tuân cùng một total order, hai concurrent operations không thể tạo cycle chỉ bằng cách đảo thứ tự hai resource. Một operation có thể phải chờ, nhưng dependency graph không tạo vòng theo cùng pattern.

## Verification plan

- Chạy workload đối nghịch với concurrency cao.
- Theo dõi database conflict/deadlock events.
- Xác nhận tổng quantity không đổi.
- Xác nhận không location nào âm.
- So sánh p95 latency trước/sau.
- Chạy soak test đủ lâu để tránh kết luận từ vài request thành công.

## Alternative fixes

### Serialize bằng một global application lock

Đơn giản nhưng phá throughput và tạo single-process coordination boundary; không phù hợp constraints hiện tại.

### Bounded retry

Có giá trị như resilience layer cho conflict hiếm, nhưng nếu giữ nguyên ordering bug thì chỉ biến failure thành latency amplification và load amplification khi burst xảy ra.

### Pessimistic locking / explicit locking hints

Có thể hữu ích trong một số workload, nhưng vẫn phải đánh giá ordering, contention, lock duration và portability. Lock mạnh hơn không tự động loại bỏ circular wait.

### Queue theo inventory key

Có thể hợp lý nếu business chấp nhận asynchronous processing và partitioning rõ ràng, nhưng tăng architecture/operations complexity đáng kể so với canonical ordering.

## Wrong / tempting fixes

- Tăng connection pool size: không giải quyết dependency cycle.
- Scale out application instances: có thể làm concurrency cao hơn và tăng xác suất conflict.
- Retry vô hạn: che symptom và có thể kéo dài incident.
- Chuyển toàn bộ workload sang một global lock: đúng về serialization nhưng không phù hợp throughput requirement.

## Production implications

Canonical ordering phải được áp dụng ở mọi code path cập nhật cùng resource set. Chỉ sửa một endpoint là chưa đủ nếu background worker hoặc batch job vẫn dùng order khác.

## Trade-offs

Ưu điểm là thay đổi nhỏ, dễ rollout, không cần nền tảng mới và giữ synchronous API. Nhược điểm là vẫn có contention khi nhiều request cùng đụng hot locations; vì vậy cần telemetry để tách contention bình thường khỏi regression.

## Senior engineer nên chú ý

Senior engineer không dừng ở việc “retry deadlock”. Họ phải phân biệt symptom recovery với root-cause control, xác định invariant, vẽ dependency graph, chọn canonical resource order, kiểm tra tất cả writers và định nghĩa rollout metrics trước khi production rollout.
