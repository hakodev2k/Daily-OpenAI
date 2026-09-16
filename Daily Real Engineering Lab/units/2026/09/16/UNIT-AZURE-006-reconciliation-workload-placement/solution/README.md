# Reference Solution — chỉ xem sau khi tự chốt decision

Đây là **một defensible solution dưới constraints đã cho**, không phải kiến trúc duy nhất đúng.

## Decision

Chọn **dedicated .NET worker chạy trên App Service riêng** cho giai đoạn hiện tại. Boundary này tách lifecycle của reconciliation khỏi web deployment nhưng vẫn dùng operational model mà team đã quen, không buộc team nhận thêm một execution model chỉ vì workload chạy theo lịch.

## Symptoms / Risks cần giải quyết

Rủi ro chính không phải CPU hay latency request. Đó là một batch dài có thể bị restart giữa chừng, chạy trùng, gặp 429 hoặc hoàn thành một phần. Nếu job sống trong web process, web deployment và scale decisions trở thành failure events của reconciliation.

## Evidence / Constraints

250k records, deadline theo giờ thay vì request latency, external rate limit 40 req/s, team nhỏ và yêu cầu không bỏ sót khiến recoverability, checkpointing và lifecycle isolation quan trọng hơn cold-start hay elastic scale.

## Processing model

- Chia workload thành deterministic batches/ranges có persisted checkpoint.
- Mỗi item có stable reconciliation key.
- Side effect phải idempotent hoặc được bảo vệ bằng durable state/unique constraint phù hợp.
- Retry 429 theo provider guidance/backoff; bounded concurrency giữ tổng rate dưới quota.
- Sau restart, worker tiếp tục từ durable state thay vì giả định process sống đến cuối.
- Một coordination mechanism ngăn nhiều scheduler owner cùng claim một batch, nhưng correctness không phụ thuộc tuyệt đối vào việc chỉ chạy một lần.

## Why this works

Hosting boundary riêng loại bỏ coupling trực tiếp với web deployment. Durable progress và idempotency giải quyết failure semantics mà việc đổi hosting service một mình không thể giải quyết.

## Verification / Acceptance

Decision chỉ được coi là đủ khi team chứng minh được các scenario: restart giữa batch, duplicate trigger, 429 kéo dài, một item fail trong batch, deployment trong cửa sổ chạy, và resume không bỏ sót/nhân đôi business effect.

## Alternatives

### BackgroundService trong web App Service

Có thể hợp lý với workload nhỏ/best-effort, nhưng ở đây web lifecycle là một failure boundary không liên quan đến business job. Có thể làm an toàn bằng durable progress, nhưng operational coupling không mang lại lợi ích rõ ràng.

### Azure Functions

Có thể là lựa chọn tốt nếu team muốn event/timer-oriented operation, scaling model và platform integration của Functions. Tuy nhiên Functions không tự giải quyết idempotency, checkpointing hay third-party quota. Chuyển service chỉ để 'job thì phải dùng Functions' là reasoning yếu.

## Wrong / Tempting Fixes

- Chỉ tăng App Service Always On: giảm một số lifecycle issues nhưng không tạo durable progress.
- Dùng distributed lock rồi coi job là exactly-once: lock không biến external side effect thành exactly-once.
- Tăng parallelism tối đa để chạy nhanh: có thể làm 429 tăng và kéo dài completion time.
- Chuyển thẳng sang Kubernetes: không phù hợp operational capability/constraint hiện tại.

## Production implications

Cần dashboard cho processed/remaining/failed/rate-limit count, structured logs với run/batch correlation, alert khi không thể hoàn tất trước deadline, và runbook cho resume/replay.

## Trade-offs

Dedicated worker thêm deployment unit và chi phí runtime riêng, nhưng đổi lại lifecycle isolation và operational clarity. Azure Functions có thể giảm một số hosting management nhưng tăng platform/runtime model mà team phải vận hành. Web-hosted worker đơn giản nhất về deployment count nhưng coupling cao nhất.

## Senior insight

Senior engineer không chọn service trước rồi mới hợp thức hóa. Họ xác định failure semantics, ownership boundary, recovery và team capability trước. Hosting product là hệ quả của decision đó.

## Revisit triggers

- Volume tăng đủ để một worker không còn đạt deadline với provider quota mới.
- Workload chuyển từ nightly batch sang event-driven gần real-time.
- Team chuẩn hóa Functions/another worker platform và có mature observability/runbook.
- Multi-region hoặc stronger availability requirements xuất hiện.