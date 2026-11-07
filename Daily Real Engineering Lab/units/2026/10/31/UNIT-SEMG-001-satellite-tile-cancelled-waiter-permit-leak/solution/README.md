# Reference Solution — SPOILER

Chỉ đọc sau khi tự reproduce và thử sửa.

## Symptoms

Khi A đang giữ một slot, B chờ và bị cancel. C có thể đi vào trước khi A kết thúc. Cuối batch C ném SemaphoreFullException.

## Evidence

Harness điều khiển thứ tự bằng TaskCompletionSource. B đã hủy, callback B không chạy. C vào sớm trước khi A nhận tín hiệu release. Lỗi không tái hiện ở hàng đợi bình thường.

## Root cause

WaitAsync(cancellationToken) có thể ném OperationCanceledException trước khi lấy được permit. Tuy nhiên finally của starter luôn Release. B vì thế tạo ra một permit không có chủ sở hữu, C nhận permit đó trong lúc A vẫn đang xử lý. Sau khi cả A và C hoàn tất, Release vượt maxCount=1.

## Why the fix works

Đánh dấu acquired=true **chỉ sau khi** WaitAsync hoàn tất thành công. Trong finally, Release chỉ khi acquired. Nếu render ném exception hoặc bị hủy sau acquisition, finally vẫn trả permit. Nếu hủy khi đang chờ, không tạo permit giả.

Thay starter/TileGate.cs bằng solution/TileGate.cs, sau đó chạy ./verify.ps1. Script verify luôn biên dịch và kiểm tra starter/, không tự chuyển sang solution/.

## How to verify

- Cancellation khi chờ: B không chạy, C không vào trước khi A hoàn tất.
- Cancellation sau acquisition: permit được trả và job kế tiếp có thể vào.
- Hàng đợi không cancellation: job thứ hai không vào trước khi job đầu hoàn tất.
- Không có SemaphoreFullException, không thay đổi harness.

## Alternative fixes

Có thể đóng gói permit thành một lease có Dispose chỉ được tạo sau acquisition thành công; hữu ích khi có nhiều điểm return và resource phức tạp. Với một hàm ngắn, cờ acquired cục bộ đơn giản hơn. Một bộ điều phối Channel/worker đơn luồng cũng có thể phù hợp nếu cần hàng đợi durable và admission control, nhưng không cần thiết chỉ để sửa lỗi này.

## Wrong / tempting fixes

- Bỏ maxCount khỏi SemaphoreSlim: che lỗi overflow nhưng cho phép vượt giới hạn thực.
- Nuốt SemaphoreFullException: giấu symptom và để capacity sai.
- Tăng số permit lên 2: làm GPU vượt khả năng.
- Không truyền CancellationToken: tránh một nhánh lỗi nhưng mất yêu cầu hủy và tăng latency.
- Bọc toàn bộ callback trong lock đồng bộ: có thể block thread, không giải quyết đúng ownership.

## Production implications

Gắn metrics cho waiting, active, cancellation-before-acquisition và release faults; log request ID và acquisition state nhưng không gắn ID có cardinality cao vào metric labels. Nếu worker bị kill hoặc render có side effect, semaphore trong process không bảo đảm điều phối giữa nhiều instance: cần cơ chế lease phân tán hoặc queue có acknowledgement phù hợp.

## Trade-offs

SemaphoreSlim nhanh, nhẹ, phù hợp giới hạn trong một process; không durable, không chia sẻ giữa replica và không tự xử lý fairness tuyệt đối. Bounded queue có backpressure rõ hơn nhưng thêm chi phí vận hành và cần policy cancellation khi đang queued.

## Senior engineer should notice

Không chỉ xác nhận exception biến mất: phải kiểm tra bất biến capacity, các đường thoát trước/sau acquisition, kiểm thử cancellation có kiểm soát không dùng sleep, và tính đúng đắn khi scale từ một process sang nhiều replica.
