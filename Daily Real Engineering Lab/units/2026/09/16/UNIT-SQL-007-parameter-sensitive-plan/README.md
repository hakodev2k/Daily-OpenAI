# UNIT-SQL-007 — Cùng query, latency khác nhau hàng trăm lần

## Mục tiêu
Điều tra một SQL Server performance incident trong đó cùng stored procedure có latency rất khác nhau tùy lịch sử execution.

## Bối cảnh thực tế
API tra cứu shipment thường trả về dưới 50 ms nhưng sau một số deployment/restart, p95 tăng lên vài giây. CPU database không tăng tương ứng và không có code application mới. DBA cung cấp execution evidence của hai thời điểm.

## Bạn cần làm gì
Dùng evidence để lập ít nhất ba hypothesis, xác định hypothesis phù hợp nhất, đề xuất cách xác nhận trên SQL Server và chọn mitigation phù hợp với data distribution cùng workload đã cho.

## Yêu cầu môi trường
Không bắt buộc SQL Server. Lab chính dùng evidence đã chụp để phân tích. Có thể dùng SQL Server local để thử extension sau lab.

## Chạy nhanh
1. Đọc `evidence/incident.md`.
2. Ghi reasoning vào `workspace/my-investigation.md`.
3. Dùng hints nếu cần.
4. So sánh với `solution/README.md`.

## Cách reproduce vấn đề
Reproduction của incident được cung cấp dưới dạng hai execution snapshots có cùng procedure và input classes khác nhau. Hãy tái dựng timeline từ evidence trước khi xem solution.

## Những gì cần quan sát
- Estimated/actual row relationships.
- Operator shape và logical reads giữa các execution.
- Data distribution của parameter.
- Sự thay đổi behavior sau compile/restart.

## Quy tắc làm lab
1. Reproduce incident từ evidence trước.
2. Ghi hypothesis.
3. Chọn evidence cần thu thêm.
4. Đề xuất fix và verification plan.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[solution/README.md](solution/README.md) — spoiler.

## Expected Results
Bạn phải giải thích được vì sao cùng procedure có thể đổi performance theo execution history, đồng thời chọn mitigation dựa trên workload thay vì áp dụng một query hint theo thói quen.

## Estimated Time
45–70 phút.