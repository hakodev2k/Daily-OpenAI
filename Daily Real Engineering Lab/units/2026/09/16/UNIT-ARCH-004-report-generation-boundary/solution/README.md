# Reference Solution — chỉ xem sau khi đã tự ra quyết định

Đây là **một phương án defensible**, không phải kiến trúc duy nhất đúng.

## 1. Symptoms
Synchronous request lifetime bắt đầu không còn phù hợp với tail workload: export có thể vượt proxy/client timeout, user retry không biết trạng thái cũ, deployment có thể cắt ngang công việc.

## 2. Evidence
Business chấp nhận 2–5 phút cho export lớn; workload tail đã tới 140 giây và dự kiến tăng; đã có timeout, duplicate retry và restart incidents. Peak concurrency vẫn nhỏ, team nhỏ và operational maturity hạn chế.

## 3. Root cause / decision pressure
Vấn đề kiến trúc không đơn thuần là query chậm. Execution lifetime của heavy work đang bị gắn vào HTTP request lifetime và web deployment lifecycle, trong khi business contract đã mang tính asynchronous.

## 4. Một decision phù hợp hiện tại
Chọn **B — durable job record + background processing trong cùng deployable**, với điều kiện implementation không phải fire-and-forget memory task.

HTTP endpoint tạo một persistent ExportJob record có idempotency/business key, trả `202 Accepted` + job id. Hosted worker claim pending jobs từ durable store, tạo artifact, cập nhật state. User poll status hoặc UI refresh; artifact lưu 24 giờ.

Lý do chưa chọn worker deployable riêng: scale hiện tại chưa chứng minh cần thêm deployable/queue infrastructure. Team có thể đạt business contract với ít operational burden hơn, nhưng thiết kế job boundary phải cho phép tách worker sau này.

## 5. Why it works
Request chỉ chịu trách nhiệm accept/validate/persist intent. Heavy work có lifecycle riêng, có trạng thái durable và có thể resume/retry sau deployment. Idempotency ngăn user retry tạo duplicate expensive work.

## 6. Reliability semantics
- Job state durable: Pending → Running → Completed/Failed.
- Claim cần concurrency-safe; stale Running job phải có recovery policy.
- Artifact chỉ publish sau khi generation hoàn thành thành công.
- Retry chỉ áp dụng transient failures và phải bounded.
- User cancellation là explicit state transition; không suy ra cancellation chỉ vì HTTP client disconnect.

## 7. Verification của decision
Trước production rollout, chứng minh bằng tests/chaos scenarios:
- submit cùng logical export hai lần không tạo duplicate heavy work;
- restart app khi job đang chạy không làm job biến mất;
- failed transient job được retry bounded;
- completed artifact tồn tại qua web restart;
- status endpoint phản ánh lifecycle chính xác.

## 8. Alternatives
### A — synchronous HTTP
Vẫn hợp lý nếu export luôn nhỏ, latency bounded dưới infrastructure timeout và business thực sự cần immediate stream. Evidence hiện tại đã làm option này yếu cho tail workload.

### C — queue + separate worker
Defensible nếu cần independent scaling, isolation khỏi web replicas, throughput lớn hơn, nhiều loại background workload, hoặc deployment independence. Đây là likely evolution path chứ chưa bắt buộc ngay.

## 9. Wrong / tempting fixes
- Chỉ tăng HTTP/proxy timeout: kéo dài coupling nhưng không giải quyết retry/status/deployment lifecycle.
- `Task.Run` rồi trả response: work không durable và lifecycle vẫn gắn với process.
- Tách microservice ngay vì “background = microservice”: tăng operational surface trước khi constraints yêu cầu.
- Retry mọi failure vô hạn: có thể khuếch đại expensive work hoặc permanent failures.

## 10. Production implications
Cần metrics cho queue depth/pending age/job duration/failure/retry, structured logs theo job id, cleanup artifact, retention policy và reconciliation cho stale jobs.

## 11. Trade-offs
Option B thêm state machine và worker coordination nhưng giữ deployment topology đơn giản. Nó không isolation tốt bằng option C và có thể tranh CPU/memory với web traffic; concurrency limits phải được đặt rõ.

## 12. Senior engineer should notice
Senior decision không phải chọn thành phần mạnh nhất. Quan trọng là nhận ra business contract đã asynchronous, tạo durable boundary nhỏ nhất đáp ứng reliability hiện tại, rồi định nghĩa **revisit triggers**.

Ví dụ triggers để chuyển sang C:
- export làm ảnh hưởng SLO của web dù đã bounded concurrency;
- queue age vượt target thường xuyên;
- cần scale worker khác web;
- nhiều background workload dùng chung durable dispatch;
- deployment/release cadence của worker cần độc lập;
- operational team đã sẵn sàng vận hành queue/worker riêng.