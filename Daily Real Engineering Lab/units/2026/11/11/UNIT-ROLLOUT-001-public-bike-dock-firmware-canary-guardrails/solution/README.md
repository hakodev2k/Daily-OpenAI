# Reference decision (spoiler)

## Evidence
Tổng fleet: 1.170/200.000 = 0,585% failed unlocks. Nhưng v3.8 riverside: 180/2.000 = **9%** (18 lần baseline v3.7 riverside 0,5%). v3.8 urban: 40/8.000 = **0,5%**, tương đương baseline urban. Fleet aggregate đánh lừa vì nhóm mới được cập nhật chỉ chiếm 5% attempts. Không thể xác nhận root cause bằng thống kê này.

## ADR
**Decision:** STOP rollout ngay, rollback cohort riverside v3.8 có kiểm soát; giữ nguyên nhóm urban chỉ nếu được phép bởi incident commander và tiếp tục quan sát. Không tăng phạm vi rollout.

**Guardrails:** Mỗi cohort version × geography phải có >=1.000 attempts/60 phút; <=1% failure mới đủ điều kiện xem xét promote; >2% hard stop tự động; bất kỳ tín hiệu safety/customer lockout nghiêm trọng nào cũng stop dù sample nhỏ. Theo dõi unlock latency p95, ticket rate, device reconnect, firmware activation success và rollback completion.

**Rollout:** 1% → 5% → 20% → 50% → 100%; mỗi stage giữ >=60 phút, đánh giá cohort-specific và device-type/geography balance, đảm bảo canary có đủ riverside exposure; giới hạn concurrent upgrade, cho phép kill switch độc lập control plane. Stage chỉ promote khi on-call SRE + device owner ký duyệt.

**Rollback:** đóng admission trước, pin version v3.7 cho nhóm bị ảnh hưởng, theo dõi acknowledgments và inventory actual-version, xác định các trạm offline để retry an toàn; so sánh post-rollback failure rate cùng cohort và time window. Nếu rollback không giảm lỗi, điều tra radio quality, local weather, configuration mismatch và firmware build; không coi rollback là bằng chứng nhân quả tuyệt đối.

## Wrong fixes
- Scale API: central 5xx không tăng; không xử lý evidence cohort.
- Dùng fleet 99,4% success: che giấu nhóm nhỏ bị ảnh hưởng.
- Rollback toàn fleet ngay: có thể không cần thiết, tăng rủi ro cho nhóm ổn định.
- Promote khi đủ 60 phút nhưng không có sample-size floor: kết luận thiếu sức mạnh thống kê.

## Trade-offs
Cohort gating làm rollout chậm hơn nhưng giảm blast radius; per-cohort metrics có thể noisy khi sample nhỏ nên cần floor và confidence check. Stop switch phải hoạt động khi control plane degraded.
