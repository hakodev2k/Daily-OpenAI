# Reference Solution — chỉ đọc sau khi tự làm

## Symptoms, evidence, hypotheses
Dashboard SUCCESS không bảo đảm media đọc được; L110 có lỗi I/O. F100 và D100 có cùng differential base 1400. D100 rồi log chain có LSN liên tục tới L109 theo fixture. Kiểm tra thực tế cần backupset, recovery fork, checkpoint và RESTORE thử; không thể quyết định chỉ từ CSV.

## Root cause và recovery boundary
L110 không đọc được nên không chứng minh được phục hồi đến 10:16:59. Tail-log chỉ hữu ích nếu có thể lấy và bao phủ liên tục LSN còn thiếu; tail-log bắt đầu sau một gap không tự lấp được gap đó. Volume mất mount khiến khả năng tail-log chưa xác định.

## Reference recovery plan
1. Cô lập writer, giữ nguyên backup media và log, chỉ restore trên standby.
2. Kiểm tra F100 và D100, restore full WITH NORECOVERY, diff WITH NORECOVERY, rồi log chain thích hợp qua L109. Đảm bảo backup metadata thực tế hỗ trợ chain; có thể không cần các log trước checkpoint của differential.
3. Tìm bản L110 lành hoặc log khác bao phủ gap. Nếu không có, chỉ xác nhận mốc restore được tới L109 sau khi thực sự restore và reconcile transaction IDs.
4. RPO: 10:17 − 10:05 ≈12 phút là proxy, **không phải chứng minh SLA**. Last recoverable transaction có thể cũ hơn 10:05; xác nhận bằng audit/sentinel transactions.
5. RTO: từ 10:31, full 19 + diff 8 + tám log ×3 + integrity 12 + smoke 8 + cutover 5 = 76 phút; kết thúc 11:47, tức 90 phút từ outage, vượt RTO 60 phút. Một số công việc có thể chạy song song nếu được chứng minh an toàn.
6. Go/no-go: integrity, row/ledger reconciliation, duplicate-safe replay, read/write smoke, auth, latency, audit, rollback, business owner sign-off nếu mất dữ liệu.

## Wrong fixes
- Tin backup SUCCESS mà không restore thử: che giấu hỏng media.
- Bỏ qua một log vì có file mới hơn: có thể phá chain.
- Bỏ integrity và smoke để kịp SLA: tăng nguy cơ data corruption.
- Tuyên bố RPO bằng backup interval: nhầm mục tiêu với recovery point thực tế.

## Trade-offs và verification
Chờ bản log thay thế có thể tăng RTO nhưng giảm mất dữ liệu; dùng replica chưa thử có thể tạo fork hoặc split-brain. Drill hàng tháng: restore trên host cô lập, sentinel transactions, checksum, immutable offsite copies, alert LSN gap, đo p95 restore duration và diễn tập cutover.

*Fixture dùng LSN đơn giản hóa, không phải hướng dẫn chạy RESTORE trực tiếp trên production.*
