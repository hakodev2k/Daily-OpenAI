# Reference Solution — SPOILER
Chỉ xem sau khi reproduce và thử sửa `starter/HeartbeatCoordinator.cs`.

## Symptoms
Một observer chạy riêng bình thường; hai observer đồng thời dẫn đến `InvalidOperationException`.

## Evidence
Hai lần gọi `ObserveAsync` được tạo trước tick đầu. Contract thứ hai yêu cầu hủy một observer mà không dừng observer còn lại.

## Root cause
`PeriodicTimer` chỉ hỗ trợ một consumer chờ `WaitForNextTickAsync` tại một thời điểm. Starter cho từng observer chờ trên cùng timer; đây không phải publish/subscribe.

## Why the fix works
[Reference code](HeartbeatCoordinator.cs) dùng một pump duy nhất đọc tick, tăng sequence ID và fan-out tới channel riêng của từng subscriber. Cancellation chỉ hủy lần đọc của subscriber đó; `finally` gỡ đăng ký. `DisposeAsync` đóng timer và chờ pump.

## How to verify
Tự sửa file trong `starter/`, giữ nguyên public API, chạy `./verify.ps1`. Chỉ sau khi thử riêng mới đối chiếu reference.

## Alternatives
- Timer riêng cho từng observer: dễ triển khai nhưng không có cùng logical heartbeat ID.
- Event callback: ít overhead hơn nhưng cần xử lý callback chậm hoặc lỗi.
- Bounded channel: kiểm soát memory tốt hơn, cần chọn drop/disconnect/backpressure rõ ràng.

## Wrong / tempting fixes
- `lock` quanh một `await`: không phù hợp; tuần tự đọc tick còn làm observer chia nhau tick.
- Nuốt `InvalidOperationException`: che lỗi và mất heartbeat.
- Tăng ThreadPool/timeout: không sửa vi phạm hợp đồng API.
- Một `Channel<long>` chung nhiều reader: mỗi tick đến một reader, không phải broadcast.

## Production implications
Reference dùng unbounded channels để giữ bài deterministic; subscriber chậm có thể gây tăng memory. Production cần bounded queue, lag metrics, unsubscribe, reconnect và quyết định delivery guarantee. `PeriodicTimer` có thể coalesce tick nếu pump bị chậm.

## Trade-offs
Fan-out bảo đảm observer đang đăng ký nhận cùng ID nhưng tăng chi phí subscription/cleanup. Cần chính sách khi subscriber bị lỗi hoặc disconnect.

## What a Senior engineer should notice
Invariant là **cùng một logical tick** cho tất cả observer, không chỉ hết exception. Kiểm chứng cancellation isolation, disposal, subscriber chậm và reconnect; phân biệt hợp đồng API với giả định của caller.

