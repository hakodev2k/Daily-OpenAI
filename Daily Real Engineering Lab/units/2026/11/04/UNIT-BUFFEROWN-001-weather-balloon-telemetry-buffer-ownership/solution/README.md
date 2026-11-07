# Reference Solution — chỉ xem sau khi tự làm

## Symptoms
Cả hai Task thành công, số bản tin đủ, nhưng bản tin A bị thay bằng nội dung B.

## Evidence
Gateway chờ một tín hiệu release trước khi đọc `ReadOnlyMemory<byte>`. Hai lệnh PublishAsync hoàn thành phần chuẩn bị payload trước khi tín hiệu được mở. Output starter: `RECEIVED: BALLOON-B:21C | BALLOON-B:21C`.

## Root cause
`TelemetryPublisher` giữ một `byte[] _frameBuffer` dùng chung. `ReadOnlyMemory<byte>` chỉ là view, không copy bytes. Lần PublishAsync thứ hai ghi đè vùng nhớ trước khi gateway đọc lần đầu. Đây là vi phạm lifetime/ownership, không phải lỗi network.

## Reference code
Trong `TelemetryPublisher`, thay field và method bằng:

```csharp
sealed class TelemetryPublisher(PausedTelemetryGateway gateway)
{
    public Task PublishAsync(string telemetry)
    {
        byte[] payload = Encoding.UTF8.GetBytes(telemetry);
        return gateway.SendAsync(payload.AsMemory());
    }
}
```

Không cần thay fake gateway hay assertions. Mỗi Task giữ tham chiếu tới vùng nhớ riêng cho tới khi send hoàn tất.

## Why the fix works
Mỗi operation sở hữu một payload không bị operation khác ghi đè. `ReadOnlyMemory<T>` không tự đảm bảo immutability, nhưng không còn writer khác lên array đó.

## Verification
`./scripts/verify.ps1` phải exit 0, output chứa cả `BALLOON-A:18C` và `BALLOON-B:21C`. `./scripts/reproduce.ps1` sau khi sửa phải exit 1 với `NOT_REPRODUCED`: script này được thiết kế để phát hiện lỗi gốc, không phải xác nhận fix.

## Alternative fixes
Dùng `ArrayPool<byte>` có thể giảm allocations nhưng phải giữ buffer cho tới khi `SendAsync` hoàn tất và chỉ Return một lần. Với nhiều writer, dùng queue chuyển quyền sở hữu rõ ràng hoặc copy payload tại boundary.

## Wrong / tempting fixes
- Thêm `Task.Delay`: che timing, không tạo ownership.
- Thêm lock chỉ quanh `Encoding.UTF8.GetBytes`: gateway vẫn đọc sau khi lock đã nhả.
- `await gateway.SendAsync(...)` trong method nhưng vẫn cho phép các call đồng thời: không tự bảo vệ shared array.
- Tăng buffer size: không ngăn ghi đè.

## Production implications
Dữ liệu bị thay mà không có exception nguy hiểm hơn lỗi rõ ràng. Cần contract về ai sở hữu buffer, lúc nào consumer hoàn tất, có được mutate hay return pool không. Log hash payload ở admission và downstream để phát hiện corruption; tránh log dữ liệu nhạy cảm.

## Trade-offs
Copy riêng tăng allocation theo frame nhưng dễ kiểm chứng và an toàn. Pooling phù hợp throughput cao sau khi có profiling và ownership contract chặt.

## Senior engineer should notice
Test phải ép send trì hoãn một cách xác định, kiểm cả nội dung chứ không chỉ Task status/count, và không suy diễn `ReadOnlyMemory` là bản sao bất biến.
