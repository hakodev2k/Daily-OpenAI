# Reference Solution — spoiler

## Symptoms
`F-101` có revision 7 ở feed forward, revision 8 ở feed reverse; số crew vẫn là 3.

## Evidence
`./reproduce.ps1` cho thấy cùng tập input nhưng output khác. Replay cùng revision cũng chọn `EventId` khác khi thứ tự thay đổi.

## Root cause
`Enumerable.DistinctBy` chọn phần tử đầu tiên cho mỗi key theo thứ tự enumeration, không tìm revision cao nhất.

## Why the fix works
[ShiftReconciler.cs](ShiftReconciler.cs) snapshot input, group theo crew, chọn revision cao nhất, tie-break replay tương đương bằng `EventId`, reject conflict cùng revision nhưng khác vessel.

## How to verify
Sau khi tự sửa starter, chạy `./verify.ps1`. Muốn đối chiếu, lưu bản của bạn rồi copy `solution/ShiftReconciler.cs` vào `starter/ShiftReconciler.cs` và chạy lại. Reproduce có thể trả exit code 1 sau fix vì bug không còn.

## Alternative fixes
Sắp xếp revision giảm dần rồi `DistinctBy` chỉ hợp lệ sau khi kiểm tra conflict. Dictionary merge revision-aware tiết kiệm bộ nhớ hơn cho batch lớn.

## Wrong / tempting fixes
`OrderBy(CrewId)` không chọn revision; `Distinct()` không giới hạn một record/crew; `Last()` vẫn phụ thuộc feed; `MaxBy` trên toàn feed làm mất crew khác; retry không đảm bảo ordering.

## Production implications
Phải xác nhận revision tăng đơn điệu theo crew. Không có revision đáng tin thì cần thay đổi upstream contract. Conflict cần telemetry và quy trình xử lý.

## Trade-offs
`GroupBy` đơn giản, cần O(n) memory; dictionary merge dùng O(k) theo số crew nhưng phức tạp. `EventId` chỉ chọn đại diện replay tương đương, không giải quyết conflict nghiệp vụ.

## Senior engineer should notice
Một test với một thứ tự feed chưa đủ. Cần permutation, replay, conflict, idempotency và contract versioning.
