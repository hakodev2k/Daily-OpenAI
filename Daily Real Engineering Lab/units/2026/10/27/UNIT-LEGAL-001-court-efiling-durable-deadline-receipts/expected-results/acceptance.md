# Acceptance contract — tự kiểm chứng thiết kế

Đây là **behavioral rubric**, không kiểm tra bằng một script chạy reference solution.

## Invariants bắt buộc

- I1: Không phát hành biên nhận thành công khi chưa có bằng chứng durable theo mức RPO đã cam kết.
- I2: Một ý định nộp (party + idempotency key + cùng canonical digest) có một submission identity và receipt ổn định qua retry; khác digest cùng key là conflict.
- I3: Verdict đúng hạn dùng thời điểm durable commit đã được định nghĩa, không dùng giờ client hay request start.
- I4: Receipt chứng minh `RECEIVED`, không tự đồng nghĩa hồ sơ được thụ lý.
- I5: Scanner/case-management down không làm mất record tiếp nhận; có retry và reconciliation quan sát được.
- I6: Partition/unknown commit không được biến thành lời hứa thành công hoặc khẳng định thất bại khi chưa tra cứu được authoritative state.
- I7: Sau failover, mọi receipt đã xác nhận phải lookup được; history append-only và audit đủ để đối soát.
- I8: Khi không thể giữ RPO=0, hệ thống fail closed và hiển thị degraded state; availability target có thể bị ảnh hưởng.

## Case acceptance

| Case | Kết quả mong đợi |
|---|---|
| A | Không còn receipt được phát hành trước durability proof; nếu mất primary trước commit thì không có confirmed receipt |
| B | Theo rule đã cho, commit sau 17:00:00.000 là **late**, dù request bắt đầu trước |
| C | Retry cùng key + digest tra cứu và trả đúng một receipt; không tạo record thứ hai |
| D | Receipt vẫn query được trong thời gian scanner down; workflow trạng thái xử lý tiến triển sau recovery |
| E | Không trả success khi durability không chứng minh được; client có hướng dẫn retry/reconcile với cùng key |

## Review rubric (0–2 mỗi mục, tổng 16)

1. Legal semantics & exact deadline instant
2. Durable acceptance boundary
3. Idempotency & ambiguous timeout
4. Replication/failover guarantees
5. State machine & downstream isolation
6. Threat model / PII / audit retention
7. Quantified trade-offs / capacity
8. Failure testing / observability / rollout

0 = bỏ sót, 1 = mô tả chung, 2 = invariant + evidence/test rõ ràng. **Pass khi >= 13/16 và không vi phạm I1–I4.** Điểm là rubric tự đánh giá, không phải cập nhật User Mastery tự động.
