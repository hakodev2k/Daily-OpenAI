# Acceptance cases
| ID | Điều kiện |
|---|---|
| A1 | DB RPO = 3 phút (10:07 − 10:04), mục tiêu ≤15 phút |
| A2 | Object replication gap = 12 phút; không suy ra mọi object đều recoverable |
| A3 | RTO = 51 phút (10:58 − 10:07), vượt mục tiêu 45 phút; health green không phải business-ready |
| A4 | P-102 thiếu object, P-103 thiếu event, P-104 ngoài DB restore, P-105 pending hợp lệ |
| A5 | Có bằng chứng primary cũ bị fenced trước khi recovery nhận ghi |
| A6 | Replay không tái cấp thuốc; remediation và notification idempotent |
| A7 | P-104 cần external evidence/manual review, không tự nhận có thể restore |
| A8 | RPO mới 2 phút: DB và object đều không đạt; nêu phương án, chi phí, rủi ro |
| A9 | Runbook có owner, go/no-go, monitoring, restore drill, abort/failback policy |

Tự đánh dấu từng case bằng chứng, không đánh đồng recovery của DB với recovery nghiệp vụ.
