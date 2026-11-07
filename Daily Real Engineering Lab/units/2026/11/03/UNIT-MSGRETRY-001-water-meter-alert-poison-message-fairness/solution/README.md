# Reference Solution — SPOILER
Chỉ đọc sau khi đã reproduce và tự thử sửa.

## 1. Symptoms
P1 bị gateway từ chối, worker dùng hết 12 ticks mà A2, B1, T1, C2 không được xử lý.

## 2. Evidence
`evidence/incident-timeline.md` cho thấy P1 lặp lại mỗi tick, delivered và dead-letter rỗng. T1 chưa được thử; 503 của T1 không phải nguyên nhân nghẽn ban đầu.

## 3. Root cause
`starter/Program.cs` luôn lấy `_pending[0]`; catch toàn bộ `Exception` nhưng không thay đổi trạng thái message, không giới hạn retries và không chọn site độc lập. Permanent failure ở đầu tạo head-of-line blocking.

## 4. Why the fix works
`solution/Program.cs` giữ mỗi message trong một envelope với attempts/next eligible tick; phân loại permanent (dead-letter ngay), transient (retry có giới hạn và deferred), unknown (propagate). Scheduler chọn message đến hạn mà không vượt qua message cũ hơn của cùng site. Site khác được phục vụ trong lúc một site chờ.

## 5. How to verify
So sánh `starter/Program.cs` với `solution/Program.cs`; sau khi tự sửa starter chạy `./verify.ps1`. Để self-check reference riêng (không thay thế verify learner), chạy:

```powershell
dotnet run --project ./solution/WaterAlerts.csproj -- --verify
```

Kỳ vọng `primary=True; exhaustion-and-fairness=True; healthy-only=True`.

## 6. Alternative fixes
Có thể dùng partitioned queue per site với scheduler round-robin và durable delayed queue; thích hợp khi throughput cao và ordering là hard requirement. Với workload nhỏ, một worker có pending state và fair selection là đủ cho local contract. Fail-fast toàn batch chỉ hợp lý khi tính toàn vẹn của toàn batch quan trọng hơn khả năng xử lý độc lập.

## 7. Wrong / tempting fixes
- Tăng tick budget: làm tăng retry của poison message nhưng không tăng progress.
- Retry tất cả exception: có thể che programming error và gây storm.
- Đẩy mọi failure xuống cuối queue mà không kiểm tra site: có thể đảo thứ tự A2 trước P1.
- Xóa P1 im lặng: mất audit và không có đường redrive.
- Thêm nhiều replicas ngay: không giải quyết retry policy và có thể nhân đôi side effects.

## 8. Production implications
Cần durable state, ACK boundary, idempotency, backoff/jitter, observability cho retry/age/dead-letter, alerting và ownership của DLQ. Sau restart, xử lý at-least-once cần deduplication. Thêm per-site concurrency cap để tránh hot site độc chiếm.

## 9. Trade-offs
Per-site ordering làm giảm parallelism trong một site nhưng giữ causal consistency. Retry budget nhỏ giúp phục hồi nhanh nhưng có thể dead-letter lỗi tạm thời dài; budget lớn tăng latency và chi phí. Dead-letter tăng vận hành nhưng bảo toàn khả năng điều tra.

## 10. What a Senior engineer should notice
Phải phân biệt successful delivery, terminal failure, deferred retry và unknown crash. Test chỉ mô phỏng deterministic tick, chưa chứng minh reliability sau crash hoặc broker redelivery. Cần SLO, incident ownership và test fault injection cho production.

**Decision:** bounded delayed retry + terminal dead-letter + fairness giữa site, giữ ordering trong site; điều chỉnh các tham số theo SLA và lỗi gateway thực tế.
