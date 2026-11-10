# UNIT-QUOTAFAIR-001 — Fairness cho gateway cảnh báo vô tuyến

## Mục tiêu
Đánh giá admission control và fairness trong hệ thống đa tenant có capacity cố định.

## Bối cảnh thực tế
Một gateway vô tuyến phục vụ RESCUE, MAINT và TEST. Sau đợt tăng tải, bản tin cứu hộ trễ dù tổng throughput vẫn đạt giới hạn thiết bị.

## Bạn cần làm gì
Đọc [scenario](scenario.md), kiểm tra bảng số liệu, ghi ít nhất 3 giả thuyết, so sánh 2 thiết kế và viết ADR với rollout/rollback, idempotency, monitoring.

## Yêu cầu môi trường
Markdown reader và PowerShell (tùy chọn), không cần cloud.

## Chạy nhanh
Đọc `scenario.md`; dùng bảng CSV trong đó để tính tổng sent theo từng phút.

## Cách reproduce vấn đề
Đối chiếu throughput, p99 và arrival của RESCUE với SLO. Phân biệt accepted và sent.

## Những gì cần quan sát
Tenant fairness, backlog, queue age, throughput, outage và duplicate retries.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints.md#hint-1), [Hint 2](hints.md#hint-2), [Hint 3](hints.md#hint-3).

## Reference Solution
**Spoiler:** [Reference Solution](solution.md) — chỉ xem sau khi tự thiết kế.

## Expected Results
ADR có capacity policy, failure semantics, 5 test scenarios, observability và rollback triggers.

## Estimated Time
55 phút.
