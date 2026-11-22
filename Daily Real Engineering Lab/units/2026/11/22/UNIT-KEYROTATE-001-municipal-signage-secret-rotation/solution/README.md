# Reference solution — chỉ đọc sau khi hoàn thành ADR

## Root cause và giới hạn chứng cứ
Client cache version `v17` tại startup; gateway chuyển ngay sang chỉ nhận `v18`, tạo khoảng lệch phiên bản. Key Vault vẫn bật `v17` không có nghĩa gateway chấp nhận `v17`. Restart có thể cập nhật cache nhưng không phải protocol rotation. Evidence 02:12 mâu thuẫn snapshot nên cần xác minh timestamp, correlation ID và tỷ lệ theo replica trước khi chốt incident report.

## Lựa chọn khuyến nghị
Dùng **versioned key acceptance** trên gateway, staged rotation với overlap tối đa 20 phút, refresh credential chủ động trên client, canary và gate theo metric. Mỗi request mang key identifier không nhạy cảm; gateway tra key tương ứng trong allowlist ngắn hạn và xác minh chữ ký constant-time. Không gửi raw key trong header/log.

### Protocol bình thường
1. **Prepare:** tạo `v18`, kiểm tra quyền Get, preload ở gateway; gateway chấp nhận `v17` và `v18` nhưng `v17` vẫn là default client.
2. **Distribute:** client refresh versionless secret theo TTL ngắn (ví dụ 2 phút ± jitter), xác minh key ID mới, canary 1 replica rồi tăng dần. Dùng atomic swap immutable credential snapshot để tránh request đọc key ID và bytes từ hai phiên bản.
3. **Observe:** theo dõi tỷ lệ 401 theo key ID/replica, cache age, Key Vault fetch failures, gateway accepted key set. Không chuyển pha nếu chưa có bằng chứng đủ replicas chuyển và error budget còn.
4. **Cutover:** client mặc định dùng `v18`; gateway tiếp tục chấp nhận `v17` trong cửa sổ overlap; sau khi xác minh không còn traffic `v17`, remove `v17` và disable version cũ ở Key Vault.
5. **Rollback:** trong overlap, quay client về `v17` khi key chưa bị compromise; gateway phải tiếp tục accept cả hai. Sau revoke, rollback về compromised key bị cấm.

### Emergency compromise
Ưu tiên chặn `v17` ở gateway ngay (trong 10 phút), chấp nhận giảm availability nếu client chưa chuyển; đẩy refresh khẩn cấp, canary, alert và incident command. Không tái bật key compromise để cứu SLA.

## Trade-offs
- Gateway chỉ nhận một key: đơn giản hơn nhưng cutover dễ gây outage.
- Gateway accept song song: tăng state và bề mặt tấn công trong thời gian ngắn; phải giới hạn overlap và audit.
- Refresh mỗi request: giảm staleness nhưng tăng latency/cost và tạo dependency Key Vault trên hot path.
- Cache TTL + background refresh + bounded last-known-good: ổn định hơn nhưng cần quản lý expiry, jitter, atomic swap và emergency invalidation.

## Verification
Dùng test harness mô phỏng từng pha: (a) mixed v17/v18 trong overlap đều 2xx, (b) v17 bị từ chối sau retire, (c) Key Vault 503 không làm mất credential hợp lệ trước expiry, (d) gateway rollback chỉ dùng key chưa revoke, (e) clock skew không tạo bypass, (f) metrics và logs không chứa raw key. Canary 1/6 rồi 2/6 rồi 6/6; halt nếu 401 >0.1% trong cửa sổ 5 phút hoặc có replica cache quá tuổi cho phép.

## Wrong fixes
- Restart toàn bộ replicas: có thể làm 401 biến mất tạm thời nhưng tạo downtime và không xử lý race lần sau.
- Tắt rotation: vi phạm security policy.
- Cache key mãi mãi: tránh request tới Key Vault nhưng không hỗ trợ revoke.
- Cho phép mọi key version vô thời hạn: kéo dài khả năng khai thác key bị lộ.
- Ghi raw secret để debug: gây rò rỉ credential.

## Rubric (100)
Evidence và kiểm tra mâu thuẫn 20; state machine + safe cutover 30; security/revoke 20; observability + canary 15; rollback/failure tests 15.
