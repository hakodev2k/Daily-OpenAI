# Self-verification rubric (không phải đáp án)

Đánh dấu PASS/FAIL kèm bằng chứng:
1. Có ít nhất 3 options và nêu khi nào mỗi option hợp lý.
2. Tính đúng 151,2 triệu events/7 năm; đơn vị KiB/GiB rõ; không gọi 251 GiB là tổng chi phí lưu trữ.
3. Có p95/p99 hiện trạng, target p95, kế hoạch benchmark với production-like dataset.
4. Phân biệt commit nghiệp vụ và ghi audit; nêu chính sách khi audit unavailable.
5. Có access control, read audit, encryption/key rotation, retention hold và erasure legal review.
6. Có integrity/tamper-evidence, backup/restore drill và tiêu chí RPO/RTO.
7. Migration có reconciliation và rollback không làm mất event.
8. Có ít nhất 3 experiments với điều kiện pass/fail định lượng.
9. Change request được đánh giá theo region failure, replication lag, index availability, cost và RTO.
10. Không tự khẳng định đã đo latency/cost nếu chỉ có giả định.

Mỗi mục 0 = thiếu, 1 = có nhưng chưa đủ bằng chứng, 2 = có thể bảo vệ. >= 16/20 và không có mục 4,5,7 bằng 0 mới đạt.