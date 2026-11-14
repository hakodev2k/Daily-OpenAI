# Reference Solution — Spoiler

## Symptoms
Ticket có severity bằng nhau bị đảo thứ tự dù số lượng không mất.

## Evidence
So sánh Expected và Actual trong replay; kiểm tra từng phần tử có severity 2.

## Root cause
.NET `PriorityQueue` không stable cho priority bằng nhau; heap chỉ đảm bảo thứ tự priority.

## Why fix works
Composite priority `(Severity, Sequence)` định nghĩa thứ tự toàn phần khi sequence duy nhất. Xem `Program.cs` bên cạnh.

## Verify
Áp dụng vào `starter/Program.cs`, chạy `./scripts/verify.ps1`; kiểm tra starter đã sửa, không phải solution.

## Alternatives
FIFO queue theo từng severity phù hợp khi tập severity nhỏ và cố định.

## Wrong fixes
`Thread.Sleep` giữa enqueue không bảo đảm thứ tự; sort sau khi công việc đã dispatch quá muộn; lock chỉ giải quyết đồng bộ truy cập.

## Production implications
Sequence cần cấp ở ingress và duy trì nhất quán nếu nhiều instance; xử lý restart, duplicate và starvation giữa severity.

## Trade-offs
Composite priority giữ O(log n), nhưng cần contract sequence rõ ràng. FIFO-per-severity dễ quan sát nhưng phức tạp quản lý queue.

## Senior engineer nên chú ý
Đừng đồng nhất ordering, fairness và concurrency; regression phải có nhiều item cùng priority.
