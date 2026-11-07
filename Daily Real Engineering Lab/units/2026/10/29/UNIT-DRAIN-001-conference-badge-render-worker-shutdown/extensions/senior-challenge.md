# Senior Challenge — đổi yêu cầu sau rollout

Bối cảnh mới: 4 replicas chạy trên Kubernetes; render p95 = 90 giây, p99 = 4 phút, termination grace = 30 giây; artifact store thỉnh thoảng ghi thành công nhưng response 503. Mỗi badge chỉ được có một artifact có thể tải, SLA p95 từ nhận job đến xuất hiện artifact = 5 phút. Không được dùng dịch vụ trả phí mới.

1. Đề xuất sơ đồ lifecycle từ SIGTERM đến job hoàn tất hoặc được redeliver; nêu rõ cửa sổ mất/nhân đôi công việc.
2. Chọn giữa kéo dài grace, chia job thành checkpoint, lease renewal và retry theo policy; lượng hóa ít nhất hai trade-offs.
3. Định nghĩa idempotency key, trạng thái artifact và cách xác minh side effect sau timeout.
4. Viết 6 acceptance tests gồm kill -9, rolling deploy, duplicate delivery, poison job, artifact-write-then-503 và broker-ack-failure.
5. Nêu metric/alert tối thiểu: age of oldest pending, in-flight lease age, redelivery count, DLQ count, missing-artifact-after-completion.

Không có một đáp án duy nhất. Bảo vệ quyết định bằng failure model, chi phí vận hành và khả năng rollback.
