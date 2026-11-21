# Reference Solution — chỉ đọc sau khi tự điều tra

## Symptoms
Hai yêu cầu được chấp nhận trong khi sức chứa là một.

## Evidence
Cả hai snapshot đều có Remaining=1, Version=1; hai thao tác ghi liên tiếp đưa remaining về 0, version lên 3.

## Root cause
Read-modify-write không kiểm tra bản ghi có thay đổi kể từ khi đọc. Ghi lần hai dựa trên snapshot cũ, gây lost update và xác nhận vượt sức chứa.

## Why the fix works
Kiểm tra đồng thời version và giá trị trước khi ghi. Một yêu cầu thành công làm tăng version; yêu cầu còn lại bị từ chối. Trong mô phỏng tuần tự, đây là compare-and-swap logic. Trong DB thực, kiểm tra và UPDATE phải **atomic**.

## How to verify
Chạy `./verify.ps1` sau khi chỉnh starter; kỳ vọng accepted=1, remaining=0, exit 0.

## Alternative fixes
EF Core concurrency token/rowversion: UPDATE kèm WHERE version cũ; 0 rows affected => DbUpdateConcurrencyException. Hoặc UPDATE có điều kiện `WHERE remaining > 0` và kiểm tra affected rows; giao dịch serializable nếu phù hợp.

## Wrong or misleading fixes
`lock` trong process chỉ bảo vệ một instance; không ngăn hai pod. Retry vô điều kiện có thể tạo xác nhận trùng. Kiểm tra remaining trước UPDATE nhưng ngoài transaction không loại bỏ race.

## Production implications
Dùng unique reservation ID, idempotency key, kiểm soát số hàng bị cập nhật và đối soát booking với capacity; test hai kết nối database độc lập.

## Trade-offs
Optimistic concurrency hợp với tranh chấp thấp nhưng cần xử lý conflict UX; pessimistic locking giảm conflict nhưng tăng contention/deadlock. Atomic decrement phù hợp trường hợp chỉ cần bảo toàn capacity.

## Senior engineer should notice
Tính đúng đắn nằm ở commit boundary, không phải ở snapshot hiển thị. Bài replay là mô hình cơ chế; không chứng minh EF Core hay SQL Server đã được kiểm thử.
