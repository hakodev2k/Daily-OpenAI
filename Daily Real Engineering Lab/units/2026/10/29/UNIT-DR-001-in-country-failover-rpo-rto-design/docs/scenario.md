# Hồ sơ quyết định DR

## Kiến trúc hiện tại
- ASP.NET Core và primary database ở Region X1 thuộc lãnh thổ X; hai availability zones nhưng cùng một region.
- 18 TB dữ liệu, bao gồm PII, backup và audit trail.
- Đỉnh tải 60 giao dịch ghi/giây; write p99 112 ms.
- Khi region mất, các thao tác đọc cache có thể tiếp tục; cấp thẻ mới chỉ được xác nhận sau durable commit.
- Phạm vi DR là mất **toàn bộ region**, không phải chỉ một zone.

## Ràng buộc bắt buộc
| Mã | Điều kiện | Bằng chứng cần |
|---|---|---|
| P1 | PII, audit, backup chứa PII phải ở trong lãnh thổ X | Data-flow audit |
| P2 | RPO tối đa 300 giây đối với write đã được xác nhận | Durable commit/replication positions khi mất primary |
| P3 | RTO tối đa 30 phút cho mất cả region | Drill đến lúc safe writes resumed |
| P4 | Write p99 ≤180 ms ở tải đỉnh | Load test |
| P5 | DR all-in ≤25.000 USD/tháng | Cost model gồm mạng, monitoring, operations |
| P6 | Không có hai primary cùng xác nhận write | Epoch/fencing proof |

Các thông số là synthetic, không đại diện SLA của bất kỳ cloud provider nào.

## Quyết định phải đưa ra
Product muốn ký GO trong hai tuần. Security không cho PII ra khỏi X. Vendor ghi “multi-region ready” nhưng chỉ đưa một số drill. Bạn có quyền từ chối sign-off nếu chưa có bằng chứng.

## Deliverables
ADR một trang, bảng pass/fail/unknown cho A–E theo P1–P6, một runbook (detect → fence → checkpoint → promote → route → verify → open writes → failback), write-admission policy, các verification gates có owner và deadline.
