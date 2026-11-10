# Requirements & constraints

## Nghiệp vụ
- Một ISBN có tối đa 2 license active trên toàn hệ thống.
- Một patron không được giữ 2 loan active cho cùng ISBN.
- Loan hợp lệ kéo dài 14 ngày, có thể trả sớm.
- Request mượn mang `operationId` ổn định qua retry; request mới phải dùng ID mới.
- API phải phân biệt `GRANTED`, `WAITLISTED`, `PENDING_RECONCILIATION`.
- Sau khi được cấp, quyền đọc cần được phát hành qua dịch vụ DRM bên ngoài.

## SLO / vận hành
- 12 chi nhánh; peak 300 requests/s trên nhiều ISBN; một ISBN hot có thể đạt 50 requests/s.
- API p95 dưới 250ms khi database khỏe; DRM đôi khi mất 3s hoặc timeout.
- Multi-instance ASP.NET Core, PostgreSQL primary một vùng, read replicas chỉ dùng cho báo cáo.
- RPO 0 cho ledger cấp quyền đã xác nhận; RTO 30 phút.
- 3 engineers, 6 tuần triển khai; không bổ sung Kafka hay Redis chỉ để giải bài này.
- Quyền đã được DRM cấp không được thu hồi tự động chỉ vì HTTP response thất lạc; phải reconcile có audit.
- Không được log patron PII trong raw telemetry.

## Invariants cần tự phát biểu
Định nghĩa chính xác active loan, trạng thái pending, capacity và quan hệ giữa loan ledger và DRM entitlement. Nêu điều kiện một response được coi là final.

## Deliverable
Architecture decision, data model tối thiểu, state machine, transaction boundary, retry/reconciliation policy, verification matrix, rollback plan và 3 metrics.
