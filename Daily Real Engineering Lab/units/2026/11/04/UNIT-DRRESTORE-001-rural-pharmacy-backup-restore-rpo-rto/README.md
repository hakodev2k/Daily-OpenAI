# UNIT-DRRESTORE-001 — Phục hồi đơn thuốc sau sự cố vùng lưu trữ

## Mục tiêu
Đánh giá disaster recovery theo RPO, RTO và tính nhất quán của PostgreSQL, object storage và message queue.

## Bối cảnh thực tế
Vùng chính của hệ thống cấp thuốc nhà thuốc nông thôn ngừng hoạt động lúc 10:07. Sau restore, một số đơn có trạng thái đã cấp nhưng thiếu PDF ký hoặc thiếu thông báo. Có giao dịch không xuất hiện trong DB phục hồi.

## Bạn cần làm gì
1. Đọc [timeline](evidence/incident-timeline.csv) và [inventory](evidence/restore-inventory.csv).
2. Tính RPO, RTO; phân loại đơn thuốc theo rủi ro.
3. Viết recovery runbook và quyết định GO/NO-GO vào [workspace](workspace/my-investigation.md).
4. Xử lý yêu cầu thay đổi: RPO từ 15 phút xuống 2 phút; RTO vẫn 45 phút.
5. Tự đối chiếu [acceptance cases](docs/acceptance-cases.md).

## Yêu cầu môi trường
Không cần tài khoản cloud, Docker hoặc dịch vụ trả phí. PowerShell tùy chọn: `Import-Csv ./evidence/restore-inventory.csv | Format-Table`.

## Chạy nhanh
Mở hai CSV, tính các mốc và ghi vào worksheet.

## Cách reproduce vấn đề
Giả lập primary mất liên lạc lúc 10:07; DB phục hồi đến WAL checkpoint, object đến replication checkpoint và queue đến offset được ghi. Join theo orderId; không mặc định exactly-once.

## Những gì cần quan sát
Sự khác nhau giữa health green và business-ready; các đơn thiếu dữ liệu xuyên kho; khoảng mất dữ liệu có thể chứng minh.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Chỉ đọc sau khi tự làm](solution/README.md).

## Expected Results
Bảng RPO/RTO, ma trận lệch dữ liệu, runbook single-writer fencing, reconciliation idempotent và tiêu chí go/no-go.

## Estimated Time
60–75 phút.
