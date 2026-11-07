# UNIT-DR-001 — Thiết kế failover nội địa: RPO, RTO và giới hạn lưu trú dữ liệu

## Mục tiêu
Đóng vai Technical Lead đánh giá Disaster Recovery (DR) qua bằng chứng diễn tập, lập Architecture Decision Record (ADR), xác định điều kiện GO/NO-GO và những giả định cần kiểm chứng.

## Bối cảnh thực tế
Nền tảng cấp thẻ vào di tích có primary database tại Region X1. Nhóm vận hành tuyên bố đã sẵn sàng chịu sự cố mất cả region trong khi Security yêu cầu toàn bộ PII luôn nằm trong lãnh thổ X. Dữ liệu diễn tập giữa các phương án dự phòng không đồng nhất.

## Bạn cần làm gì
1. Đọc docs/scenario.md và evidence/data-policy.md.
2. Tự kiểm tra số liệu evidence/failover-drills.csv, phân biệt observed với guaranteed.
3. So sánh tối thiểu ba phương án về residency, RPO, RTO, write latency, cost, fencing.
4. Điền workspace/decision-record.md, đề xuất runbook và quyết định GO/CONDITIONAL GO/NO-GO.
5. Dùng verification/acceptance-checklist.md kiểm tra trước khi đọc reference.
6. Xử lý thay đổi yêu cầu ở extensions/requirement-change.md.

## Yêu cầu môi trường
Markdown reader, bảng tính tùy chọn; không cần Azure, Docker, database hoặc quyền production. Mọi dữ liệu đều synthetic.

## Chạy nhanh
Mở docs/scenario.md → evidence/failover-drills.csv → workspace/decision-record.md → verification/acceptance-checklist.md.

## Cách reproduce vấn đề
Đây là Architecture Decision Lab, không có code lỗi để chạy. Tái hiện vấn đề bằng cách kiểm tra từng drill và từng ràng buộc cứng thay vì tin vào tuyên bố tổng kết.

## Những gì cần quan sát
Đối chiếu phạm vi sự cố, độ trễ replication, RTO end-to-end, latency, chi phí và quyền lưu PII. Phân biệt một kết quả đo tốt với một cam kết bảo đảm được.

## Quy tắc làm lab
1. Reproduce bằng số liệu.
2. Ghi ít nhất ba hypotheses.
3. Đề xuất quyết định và guardrails.
4. Verify với checklist.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ đọc sau khi hoàn tất ADR:** [Reference](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
80 phút — L5 / D4. [Workspace](workspace/decision-record.md).
