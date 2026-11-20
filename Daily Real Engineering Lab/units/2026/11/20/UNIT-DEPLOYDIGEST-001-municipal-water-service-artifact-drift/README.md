# UNIT-DEPLOYDIGEST-001 — Cùng release, khác binary

## Mục tiêu
Điều tra sự không nhất quán của release giữa hai vùng; thiết kế phương án containment, rollback và artifact promotion có thể kiểm chứng.

## Bối cảnh thực tế
Dịch vụ giám sát xử lý nước có hai vùng A/B cùng hiển thị release 2026.11.20 nhưng cùng input trả về hai giá trị khác nhau. RTO 15 phút; không được mất dữ liệu đo.

## Bạn cần làm gì
Đọc [timeline](evidence/timeline.md), lập ít nhất ba giả thuyết cạnh tranh, tìm evidence phân biệt, viết quyết định ứng phó và pipeline tương lai vào [workspace](workspace/decision.md). Đánh giá bằng [rubric](expected-results/acceptance.md).

## Yêu cầu môi trường
Markdown reader; không cần cloud account hoặc Docker daemon. Đây là tabletop dựa trên evidence cố định.

## Chạy nhanh
Mở evidence và ghi quyết định vào workspace trong 45 phút.

## Cách reproduce vấn đề
Đối chiếu hai runtime imageID và cùng release label trong timeline; phân loại sự khác biệt có thể quan sát. Không có executable production.

## Những gì cần quan sát
Sự khác nhau giữa UI release label, registry state và runtime identity; độ tin cậy của rollback target và kiểm soát dữ liệu.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution](solution/README.md).

## Expected Results
[Acceptance](expected-results/acceptance.md)

## Estimated Time
45 phút.
