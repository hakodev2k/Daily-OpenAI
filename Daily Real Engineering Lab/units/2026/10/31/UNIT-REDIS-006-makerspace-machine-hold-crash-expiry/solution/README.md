# Reference Solution — chỉ xem sau khi tự reproduce và thử fix

## Symptoms

Yêu cầu bình thường vẫn có thời hạn giữ chỗ, nhưng một lần client process bị kill làm machine slot không thể được cấp lại ngay cả sau khi thời gian giữ chỗ dự kiến đã qua.

## Evidence

`HOLD_EXISTS_AFTER_RESTART=True` và `FINITE_TTL_AFTER_RESTART=False` chứng minh key tồn tại mà không có hạn sử dụng. Sau khi `ManualClock` tiến 3 phút, retry vẫn bị từ chối. Nhánh bình thường không lỗi, giúp khoanh vùng crash boundary.

## Root cause

Starter thực hiện hai thao tác độc lập: tạo key có điều kiện rồi mới đặt TTL. Nếu process chết sau thao tác thứ nhất, key được lưu bền nhưng thiếu expiry. Không có transaction bảo đảm cả hai thao tác cùng thành công.

## Why the fix works

Mã trong [ReservationLeaseService.cs](ReservationLeaseService.cs) dùng `SetIfAbsentWithExpiry`: điều kiện cấp key và TTL là một thay đổi atomic. Crash injection chạy **sau** thao tác atomic, nên key vẫn có TTL. Simulator cấm ghi thêm từ client đã chết, tránh giả định sai rằng `finally` luôn cứu được trạng thái.

Trong Redis thật, hợp đồng tương ứng là một lệnh `SET hold:{slot} {requestId} NX EX 120` (hoặc `PX` với millisecond). Không dùng `SETNX` rồi `EXPIRE` như hai lệnh riêng.

## How to verify

Từ thư mục unit, sao chép `solution/ReservationLeaseService.cs` sang `starter/ReservationLeaseService.cs`, sau đó chạy `verify.ps1`. Chỉ thao tác trên bản làm việc của mình; không thay đổi unit đã publish trên GitHub. Kỳ vọng `VERIFICATION_PASSED`, cả crash path và normal path đều đúng.

## Alternative fixes

- Redis Lua script có thể thực hiện conditional write + expiry atomically, nhưng một lệnh `SET NX EX` đơn giản hơn và ít vận hành hơn cho hợp đồng này.
- Database row với unique constraint và `expires_at` có thể thay Redis khi dữ liệu đặt lịch cần transaction chung với hệ thống nguồn; cần xử lý concurrency và cleanup hợp lý.
- Với hệ thống lớn, xác nhận reservation cuối cùng trong authoritative DB, không coi lease key là bằng chứng thanh toán/đặt lịch đã hoàn tất.

## Wrong or misleading fixes

- `try/finally` gọi `Expire`: process crash không bảo đảm `finally` chạy. Simulator cố ý mô phỏng điều này.
- Cron job dọn key thiếu TTL: có thể là cơ chế khắc phục lịch sử, nhưng không bảo đảm không có orphan mới và cần quy tắc ownership.
- Tăng TTL hoặc retry nhiều lần: không giải quyết key tồn tại vô hạn.
- Chỉ gọi `Expire` trước `SetIfAbsent`: chưa có key thì không có gì để expire.

## Production implications

- Redis command có thể commit nhưng client timeout trước khi nhận response: kết quả **unknown**, không nên tự động suy luận “chưa tạo hold”.
- Dùng request ID ổn định để reconcile, có thể đọc lại key và so sánh ownership khi cần idempotency.
- Lease expiration không phải cơ chế đảm bảo mutual exclusion tuyệt đối khi holder cũ còn chạy; thao tác ghi tài nguyên cần fencing/transaction nếu có rủi ro stale owner.
- Redis durability, eviction, failover, clock và chính sách TTL cần được đánh giá riêng khi triển khai thật.

## Trade-offs

Atomic Redis lease có độ trễ thấp, nhưng là trạng thái tạm thời, không thay thế transaction đặt lịch. Database transactional claim tăng độ chặt chẽ cho nghiệp vụ cuối cùng nhưng có chi phí ghi và điều phối cao hơn.

## What a Senior engineer should notice

Sự khác biệt giữa *hai lệnh thành công khi không crash* và *một bất biến còn đúng sau crash*. Thiết kế phải nêu rõ linearization point, trạng thái unknown sau network failure, thời hạn ownership và nơi lưu sự thật nghiệp vụ. Probe dùng simulator để cô lập nguyên lý; vẫn cần integration test với Redis thật trước khi production.

## References

- Redis SET command: https://redis.io/docs/latest/commands/set/
- Redis EXPIRE command: https://redis.io/docs/latest/commands/expire/
- Microsoft .NET 8: https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8/overview
