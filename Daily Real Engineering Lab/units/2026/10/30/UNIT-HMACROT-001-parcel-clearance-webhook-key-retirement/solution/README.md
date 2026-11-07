# Reference Solution — SPOILER

**Chỉ xem sau khi tự hoàn thành ADR.** Đây là một quyết định có thể bảo vệ trong điều kiện synthetic, không phải thiết kế duy nhất.

## 1. Symptoms
E103/E105 hợp lệ tại thời điểm phát hành nhưng chưa được xử lý do 504/503; retry K1 sau 09:30 bị 401. Parcel không được release dù dashboard HTTP success phần lớn là xanh.

## 2. Evidence
- K1 được dùng cho event mới đến 09:12; Security cutoff 09:30.
- E103: 09:08 → 09:40 = **32 phút**; E105: 09:11 ngày 30 → 08:50 ngày 31 = **23 giờ 39 phút**.
- E101/E102 được xử lý K1 trước cutoff. E104/E106 được xử lý K2. E106 duplicate không tạo transition mới.
- E107 signature invalid, E108 timestamp trước thời điểm createdAt 15 phút theo evidence; không chấp nhận chỉ vì keyId=K2.
- Partner retry giữ nguyên signature; không có re-sign contract. Feed qua OAuth tồn tại nhưng completeness chưa được chứng minh.

## 3. Root cause / feasibility
Không thể đồng thời đạt hai hard requirements bằng **webhook-only**: (i) không verify K1 sau 09:30 và (ii) xử lý mọi first successful delivery có thể đến muộn tới 24h nhưng vẫn ký K1. Mở rộng cửa sổ K1 vi phạm Security; đóng đúng giờ làm mất late first deliveries. HTTP 2xx, eventId hoặc inbox record không tạo ra trust mới.

## 4. One defensible decision
**NO-GO cho webhook-only. CONDITIONAL GO cho hard cutoff + authenticated authoritative-feed reconciliation**, với các gate:
1. Phân phối K2 tới cả ba replica trước 09:00; canary K2 và kiểm tra config version.
2. Chặn K1 verification sau 09:30 trên mọi replica; fail closed nếu config không đồng bộ.
3. Pull feed mỗi 5 phút với OAuth2 client credentials, TLS, tenant scope; cursor checkpoint và pagination idempotent; không coi event từ rejected webhook là authenticated.
4. Chỉ ghi parcel transition từ (a) webhook đã verify bằng key hợp lệ tại thời điểm nhận, hoặc (b) event lấy từ authoritative feed đã xác thực độc lập.
5. Upsert durable inbox theo `(partnerId,eventId)` và digest. Same digest → no-op; different digest → quarantine/alert. Transaction bao gồm inbox/parcel transition; cursor chỉ advance sau khi batch xử lý bền vững.
6. Chỉ ký GO sau khi chứng minh feed **đầy đủ**, có khả năng phục hồi gap, 24h bound ở peak/outage, và được Security/Product sign-off. Nếu feed không đạt, giữ NO-GO và đàm phán re-sign/replay API.

## 5. Why this works conditionally
Cutoff giữ ranh giới trust: không cần K1 sau 09:30. Feed là một đường khôi phục độc lập có xác thực, không phải bypass signature. Với retention 48h và delay thông thường 2m, polling 5m có tiềm năng xử lý E103/E105 trong 24h; **không phải guarantee** khi chưa test lag tails, cursor loss và feed completeness.

## 6. How to verify
- Replay toàn bộ CSV, thêm negative cases: forged K1 after cutoff, tampered K2, skew >5m, duplicate ID khác body, feed tenant mismatch.
- Inject feed outage, missing page, cursor expiry, 3-replica rolling restart, retry after cutoff và dual arrival (webhook + feed).
- Assert K1 accepted count after cutoff = 0; unique parcel transitions per eventId = 1; no untrusted payload promoted.
- Đối chiếu event IDs/cursors với partner authoritative inventory; đo oldest unprocessed event age và worst-case recovery <=24h dưới fault envelope đã phê duyệt.
- Runbook nêu owner, stop/go threshold, alert, rollback không mở lại K1, và escalation khi reconciliation không bảo đảm bound.

## 7. Alternatives
| Option | Security cutoff | 24h recovery | Nhận định |
|---|---|---|---|
| A. Giữ K1 24h | FAIL | Có thể PASS | Không hợp lệ nếu hard cutoff không đổi |
| B. Cutoff 09:30, webhook-only | PASS | FAIL | E103/E105 bị bỏ lỡ |
| C. Cutoff + authenticated feed | PASS về thiết kế | CONDITIONAL | Cần proof completeness và lag |
| D. Partner re-sign retries bằng K2 | PASS nếu contract mới | CONDITIONAL | Đòi partner change + staging tests; vẫn cần dedup |

## 8. Wrong / tempting fixes
- “Cho K1 grace 24h”: vi phạm hard retirement, mở rộng thời gian tin khóa cũ.
- “Nhìn 99.9% webhook 2xx”: percentile che tail và first-delivery loss.
- “Tin eventId của request 401 rồi tự apply”: eventId không chứng minh authenticity.
- “Đọc JSON rồi ký lại”: khác raw-body HMAC contract, có thể làm mất khả năng verify.
- “Lưu cursor trước khi commit”: crash làm mất event; phải kiểm soát commit boundary.
- “Thêm Redis để dedup”: không tự giải quyết trust hoặc atomic state transition.
- “Retry nhiều hơn”: chữ ký K1 vẫn không hợp lệ sau cutoff.

## 9. Production implications
Cần key lifecycle audit, secret distribution readiness, monotonic config version, inbox retention >= partner replay window + margin, authenticated feed ownership, rate-limit/backoff, cursor gap alarms, reconciliation drill, manual review cho digest mismatch và incident playbook khi partner mất feed.

## 10. Trade-offs
Feed tăng complexity, read cost, operational dependency và tạo thêm trust boundary, nhưng tách availability của webhook khỏi tính đầy đủ business events. Re-sign retries đơn giản hơn cho receiver nhưng phụ thuộc partner contract. Với hard security cutoff, chọn hệ thống có thể chứng minh bằng test thay vì lặng lẽ kéo dài trust.

## 11. What a Senior engineer should notice
Phải nhận ra **mâu thuẫn hợp đồng**, không nhầm retry delivery với new event, không nhầm acceptance với business correctness, phân biệt claimed SLA và observed bound, và không ký GO khi thiếu evidence về completeness.

## 12. Requirement change
Feed có thể lag **26h**, nên phương án C không còn chứng minh 24h recovery. **NO-GO** nếu không có kênh mới có SLA <=24h, ví dụ partner re-sign retry, authenticated low-latency replay/backfill endpoint hoặc thay đổi hard constraint được phê duyệt. Không tự nới K1 cutoff. Escalate Product/Security, ghi rõ số parcel bị chậm và vận hành manual hold; không tự release parcel không có bằng chứng xác thực.
