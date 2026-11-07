# UNIT-SQL-002 — Hai giao dịch đều đúng, quy tắc nghiệp vụ vẫn bị phá vỡ

## Mục tiêu
Điều tra lỗi concurrency trong SQL Server nơi từng transaction đều commit nhưng trạng thái cuối vi phạm business rule.

## Bối cảnh thực tế
Hệ thống trực bệnh viện yêu cầu mỗi khoa luôn còn ít nhất một bác sĩ `OnCall`. Hai bác sĩ có thể đồng thời xin rời ca trực.

## Bạn cần làm gì
Reproduce bằng hai terminal, ghi thứ tự sự kiện và trạng thái cuối, đưa ra ít nhất 2 hypotheses, sửa scripts trong `starter/` và verify.

## Yêu cầu môi trường
SQL Server local/container và `sqlcmd`.

## Chạy nhanh
Chạy `setup.sql`; mở hai terminal; chạy `session-a.sql`, ngay sau đó `session-b.sql`; cuối cùng chạy `inspect.sql`.

## Cách reproduce vấn đề
Hai session mô phỏng hai request hợp lệ xảy ra gần như đồng thời. Nếu reproduce thành công, cả hai báo thành công nhưng kiểm tra cuối phát hiện business invariant bị vi phạm.

## Những gì cần quan sát
Ghi giá trị mỗi session đọc trước update, thời điểm begin/commit, blocking nếu có và trạng thái cuối bảng. Phân biệt điều database đảm bảo với điều business rule yêu cầu.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[⚠️ Spoiler](solution/README.md)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
55 phút.