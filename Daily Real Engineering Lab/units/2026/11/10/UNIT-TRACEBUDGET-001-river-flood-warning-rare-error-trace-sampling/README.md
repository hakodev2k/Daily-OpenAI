# UNIT-TRACEBUDGET-001 — Thiết kế trace sampling cho cảnh báo lũ hiếm lỗi

## Mục tiêu
Đánh giá một quyết định observability dưới giới hạn ngân sách, độ trễ và dữ liệu nhạy cảm; phân biệt metrics, logs và distributed traces.

## Bối cảnh thực tế
Dịch vụ phát cảnh báo lũ xử lý 120.000 request/phút. Dashboard báo lỗi dưới 0,2%, nhưng nhóm trực không tìm thấy trace hoàn chỉnh cho một số cảnh báo thất bại. Không thể bật lưu 100% traces lâu dài.

## Bạn cần làm gì
1. Đọc [incident evidence](evidence/incident.md) và [policy candidates](docs/decision-worksheet.md).
2. Tính số traces được giữ, tỷ lệ bắt lỗi và ngân sách trong từng phương án.
3. Đề xuất chính sách sampling và rollout có kiểm soát, kèm rollback.
4. Đối chiếu [reference decision](solution/README.md) chỉ sau khi chốt lựa chọn.

## Yêu cầu môi trường
Chỉ cần Markdown và máy tính; không yêu cầu cloud, credentials hoặc triển khai production.

## Chạy nhanh
Mở `evidence/incident.md`, tính theo bảng dữ liệu và điền `workspace/my-investigation.md`.

## Cách reproduce vấn đề
Dùng cùng bộ 1.000.000 requests trong evidence; tái tính số error traces được giữ theo sampling rule của từng phương án, không thay đổi tập dữ liệu.

## Những gì cần quan sát
So sánh số traces chứa lỗi, chi phí lưu trữ, khả năng ghép nối các span và ảnh hưởng đến p99 của luồng cảnh báo.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix hoặc ra quyết định.
4. Verify bằng các invariants.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Decision, calculations and wrong fixes](solution/README.md)

## Expected Results
Một ADR ngắn, có phép tính kiểm chứng, retention budget, điều kiện triển khai và rollback; không tuyên bố bắt được 100% lỗi nếu không chứng minh.

## Estimated Time
55 phút.
