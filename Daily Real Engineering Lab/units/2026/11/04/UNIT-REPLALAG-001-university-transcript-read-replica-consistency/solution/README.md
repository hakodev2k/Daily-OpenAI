# Reference Solution — chỉ đọc sau khi tự phân tích

## Symptoms
A/D trả 200 với revision 11 thay vì 12; C trả 404 dù primary đã commit. B/E hợp lệ. F là degradation khi primary không sẵn sàng; cần định nghĩa rõ thay vì giả định 503 luôn là bug.

## Evidence
Commit thành công ở primary; bản sao được đọc với revision khác nhau. p95 HTTP tốt không chứng minh freshness. Lag p99 11s vượt ngưỡng 2s.

## Root cause
Replica cập nhật bất đồng bộ; đường download yêu cầu read-your-writes nhưng đọc bản sao eventual consistency mà không mang minimum revision/commit fence. Không thể kết luận từ CSV rằng toàn bộ replica luôn lỗi.

## Một quyết định khả thi
Sau thanh toán, trả một opaque signed consistency token gắn studentId, transcriptId, minimumRevision, expiry; server cũng có thể lưu required revision trong session/authoritative state. Download kiểm tra quyền trên server và so sánh revision. Với request có yêu cầu freshness, đọc primary trực tiếp trong cửa sổ ngắn hoặc khi replica chưa đạt revision; chỉ trả 200 nếu revision >= minimumRevision. Khi không thể xác minh do primary outage, trả 503 + Retry-After hoặc 202 + trạng thái pending có hợp đồng rõ ràng; tuyệt đối không trả 200 với bản cũ. Request đọc lịch sử thông thường tiếp tục replica. Token không thay thế authorization và không được tin nếu chưa xác thực chữ ký và scope.

## Vì sao hiệu quả
Tách nhóm 4% đọc nhạy cảm khỏi 96% đọc thường. 4% của 3,000 RPS = 120 RPS, dưới 350 RPS headroom nếu phân bố phù hợp; phải đo burst thực tế và giới hạn concurrency trước rollout. Cần cân nhắc nếu payment-confirmed download traffic tập trung hơn giả định.

## Verification
- A/C/D: không 200 stale và không 404 giả; khi primary khỏe, đọc revision >= 12.
- B/E: revision 12, không tăng lỗi.
- F: fail closed theo freshness contract, có retriable signal; không fallback trả stale.
- Cross-student token: 403 hoặc 404 theo security policy, không lộ transcript.
- Repeated request / retry: không ghi thanh toán lần nữa, không sinh side effect.
- Telemetry: `required_revision`, `served_revision`, `read_source`, `replica_lag_ms`, `freshness_fallback`, `fallback_latency` với student identifiers được bảo vệ.

## Alternatives
1. All-primary: đơn giản, nhất quán hơn cho reads nhưng 3,000 RPS vượt headroom 350 RPS; không phù hợp constraint.
2. All-replica + retry: giảm primary load nhưng không bảo đảm <=2s tại p95 lag 2.4s và p99 11s; không được trả 200 stale.
3. Targeted primary read: cân bằng đúng đắn và chi phí; cần token lifecycle, observability và rate limiting.
4. Wait for replica to catch up: khả thi cho SLA dài hơn hoặc lag thấp, nhưng phải có timeout/fallback và load tests; không phải mặc định tốt hơn.

## Tempting / wrong fixes
- `Task.Delay(500)` cố định: phụ thuộc tail lag, không có guarantee.
- Cache bản điểm 60 giây: có thể kéo dài stale nếu không có revision gate.
- Retry 404 không giới hạn: tăng tải và gây timeout; phải có deadline và jitter phù hợp.
- Bật synchronous replication ngay: thay đổi latency/availability của write path, vượt ngân sách hiện tại.
- Tăng replica CPU ngay: chưa chứng minh bottleneck là CPU.

## Production implications & trade-offs
Rollout 5% → 25% → 100% của download-after-payment; theo dõi stale-200 count (phải bằng 0), 503/202 rate, primary read QPS, p99, payment write latency và cross-tenant access. Rollback chỉ về cơ chế an toàn không trả stale; có thể tắt primary fallback và trả retriable error, không rollback về silent stale. Thực hiện capacity test peak/burst.

## Senior engineering observations
Xác định consistency SLA trên từng endpoint; phân biệt commit acknowledgment, read visibility và user-facing correctness. Thiết kế failure semantics trước khi tối ưu latency. Thử nghiệm bằng revision assertions chứ không chỉ status code; đo load headroom theo worst-case chứ không chỉ trung bình.
