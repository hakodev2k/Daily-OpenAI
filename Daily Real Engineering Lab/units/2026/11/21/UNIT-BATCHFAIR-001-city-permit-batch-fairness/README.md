# UNIT-BATCHFAIR-001 — Hàng đợi xử lý hồ sơ: nhanh nhưng không công bằng

## Mục tiêu
Thiết kế chính sách điều phối công việc nền có thể kiểm chứng bằng dữ liệu, thay vì chỉ tăng số worker.

## Bối cảnh thực tế
Nền tảng cấp phép xây dựng xử lý hồ sơ của 12 quận. Một quận gửi hàng chục nghìn hồ sơ theo lô; các quận còn lại báo cáo hồ sơ khẩn cấp chờ hàng giờ, dù tổng throughput vẫn cao. Hệ thống hiện có một queue FIFO và 8 worker.

## Bạn cần làm gì
Phân tích trace trong [evidence/queue-trace.csv](evidence/queue-trace.csv), thiết kế ít nhất hai phương án điều phối, viết ADR trong [workspace/decision.md](workspace/decision.md), và mô phỏng thứ tự xử lý bằng bảng tính hoặc script của bạn. Giữ throughput tối thiểu 8 công việc/phút; hồ sơ khẩn của quận nhỏ phải bắt đầu trong 3 phút ở tải mẫu; không làm mất hoặc nhân đôi công việc.

## Yêu cầu môi trường
Chỉ cần trình đọc CSV và trình soạn thảo; không cần cloud, Docker hay tài khoản Azure.

## Chạy nhanh
PowerShell: `Import-Csv ./evidence/queue-trace.csv | Format-Table`

## Cách reproduce vấn đề
Giả lập một worker xử lý một công việc/phút, 8 worker song song, FIFO theo thời điểm đến; thời gian xử lý từng việc là 1 phút. Xem thời điểm bắt đầu của các hồ sơ khẩn đến sau đợt batch.

## Những gì cần quan sát
Đo thời gian chờ theo quận, p95 tổng thể và p95 theo lớp khẩn; đối chiếu throughput và độ trễ của từng nhóm. Không suy ra nguyên nhân từ throughput tổng.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử thiết kế.
4. Verify bằng replay.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Phân tích và các phương án](solution/README.md) — chỉ mở sau khi tự ra quyết định.

## Expected Results
[Trước và sau](expected-results.md). Bài lab không yêu cầu một kiến trúc duy nhất; cần chứng minh giới hạn starvation, độ trễ và chi phí vận hành.

## Estimated Time
45–60 phút.
