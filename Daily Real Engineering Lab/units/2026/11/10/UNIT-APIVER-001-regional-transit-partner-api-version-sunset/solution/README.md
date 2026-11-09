# Reference Solution — chỉ đọc sau khi tự làm

Đây là **một phương án có thể bảo vệ được**, không phải đáp án kiến trúc duy nhất.

## Symptoms và evidence
Tắt v1 T+21 khiến K03 không gửi được 900 receipt khi online lại T+34. K04 không có traffic trong 7 ngày nhưng vẫn có thể hoạt động T+50. K05 chưa có owner; không thể xác nhận migration. p95 v2 thấp hơn không chứng minh lợi ích đủ để chấp nhận mất giao dịch.

## Root cause của rủi ro
Lấy ngày deadline và traffic 7 ngày làm bằng chứng toàn bộ consumer đã chuyển đổi; đồng thời thiếu contract identity mapping an toàn. Gateway rename fields không giải quyết uniqueness vì ticketId có thể tái sử dụng.

## Decision đề xuất
Chọn B có điều kiện, kết hợp **compatibility v1 endpoint giới hạn consumer** và kế hoạch sunset theo bằng chứng; giữ v1 ít nhất qua T+60 cộng khoảng thời gian offline/replay đã được kiểm chứng. Không công bố ngày cắt tuyệt đối cho K03 trước khi có firmware và backlog reconciliation. Cho K01 cutover sớm, K02 sau khi có duplicate reconciliation; K04 cần xác nhận từ owner và test lúc kỳ học mở lại; K05 phải truy được owner hoặc thu hồi token qua quy trình kiểm soát, không xóa mù.

## Vì sao hiệu quả
Tiếp nhận backlog theo đúng contract gốc, giữ identity và idempotency ở biên lưu receipt. Với kiosk cũ, khóa idempotency phải bao gồm identity ổn định của thiết bị và sequence/generation có thể xác minh; nếu protocol không có identity đủ mạnh, đưa giao dịch nghi vấn vào reconciliation queue thay vì tự chế transactionId từ ticketId. Audit mọi quyết định.

## Go/no-go gates
- 100% clientId được gắn owner, xác nhận active/dormant và lịch offline tối đa.
- Traffic theo clientId/version/status; kiểm tra 4xx, 5xx, retry, queue age, receipt reconciliation; không có giao dịch chưa đối soát.
- Consumer-specific signed migration acceptance và synthetic replay cho trường hợp offline 14–30 ngày.
- K03 900 receipt được xử lý đầy đủ, không mất/nhân đôi; K04 có test thực tế sau T+50.
- Rollback drill chứng minh bật route cũ trong 30 phút, database vẫn tương thích.
- Product, partner manager và compliance ký go/no-go; SRE sở hữu dashboard, backend sở hữu adapter và idempotency.

## Cutover / rollback
T+0 instrument và inventory; T+7 canary K01; T+14 K02 với reconciliation; T+21 **không tắt toàn bộ v1**; T+34 đối soát backlog K03; T+50 test K04; sau T+60 cập nhật readiness K03, gia hạn có thời hạn nếu cần. Khi mismatch, 5xx spike, unknown consumer hoặc backlog không thể ingest: dừng sunset, khôi phục route còn tương thích, giữ receipt log và mở incident.

## Alternative solutions
A chỉ hợp lý nếu tất cả consumers có bằng chứng migrated và không còn backlog, trái dữ liệu hiện tại. C hợp lý nếu bổ sung stable identity protocol, idempotency mapping và contract tests; chỉ đổi tên field không đủ. B tăng chi phí hỗ trợ (45 USD/ngày giả định) và complexity nhưng tránh rủi ro compliance lớn hơn. Có thể dùng v1 facade thay vì duy trì toàn bộ backend nếu đã chứng minh correctness.

## Tempting / wrong fixes
- '0 request trong 7 ngày = inactive': bỏ sót K04.
- 'Chuyển đổi gateway tự động': ticketId tái sử dụng gây collision.
- 'Tắt rồi bật lại nếu lỗi': backlog không nhận được và migration schema có thể khiến rollback bất khả thi.
- 'Tăng retry': không chữa lỗi contract, có thể làm duplicate nặng hơn.

## Production implications / Senior signals
Phải kiểm tra consumer inventory, telemetry coverage, retention, idempotency, audit, owner và rollback trước khi quyết định ngày sunset. Tối ưu chi phí 45 USD/ngày không được đánh đổi mất receipt. Phân biệt compatibility ở HTTP layer với semantic compatibility của giao dịch; ưu tiên đo và chứng minh hơn là lựa chọn công nghệ cầu kỳ.

## Verification
Chạy timeline T+21, T+34, T+50 với mọi consumer, đối chiếu 8 tiêu chí trong rubric; quyết định phải giải thích từng receipt và mọi ngoại lệ. Không có runtime code trong bài này; kiểm chứng là tabletop + contract/rollback test plan, không phải test đã thực thi.
