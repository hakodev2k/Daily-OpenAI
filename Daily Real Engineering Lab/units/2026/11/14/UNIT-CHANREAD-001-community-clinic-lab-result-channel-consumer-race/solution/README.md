# Reference Solution — chỉ đọc sau khi tự reproduce và sửa

## Root cause
`WaitToReadAsync` trả `true` khi dữ liệu có thể đọc, không cấp quyền sở hữu item. Hai consumer có thể cùng thấy `true`; chỉ một consumer lấy được item. `TryRead` trả `false` cho consumer còn lại là kết quả hợp lệ, không phải lỗi queue. Channel đã complete nên worker đó phải kết thúc bình thường.

## Fix
Thay hàm `Worker` trong starter bằng:

```csharp
async Task Worker(int workerId)
{
    var firstWait = true;
    while (await channel.Reader.WaitToReadAsync())
    {
        if (firstWait)
        {
            firstWait = false;
            if (Interlocked.Increment(ref arrived) == 2) gate.TrySetResult();
            await gate.Task;
        }
        if (!channel.Reader.TryRead(out var result)) continue;
        Interlocked.Increment(ref processed);
        Console.WriteLine($"PROCESSED worker={workerId} id={result}");
    }
    Console.WriteLine($"DONE worker={workerId}");
}
```

Lần đầu hai reader đi qua gate, một reader lấy item, reader kia `TryRead` thất bại và quay lại `WaitToReadAsync`. Vì writer đã complete và không còn item, nó nhận `false` rồi kết thúc. `verify.ps1` chạy chính starter đã sửa và yêu cầu `processed=1, errors=0`.

## Verification và trade-offs
Giữ `SingleReader=false` khi có hai worker. `ReadAllAsync` là API thay thế hợp lệ khi không cần xử lý trực tiếp `TryRead`; cần thiết kế lại replay gate nếu đổi API. Với production, bỏ replay gate, ghi metrics xử lý thành công/lỗi nghiệp vụ, và bảo đảm side effect idempotent nếu job có thể được gửi lại từ upstream.

## Wrong fixes
- Đặt `SingleReader=true` trong khi có hai reader: khai báo sai contract, không sửa cạnh tranh.
- `Task.Delay` trước `TryRead`: chỉ thay đổi lịch chạy, không tạo quyền sở hữu.
- Bắt mọi exception rồi bỏ qua: che mất lỗi xử lý thật.
- Thêm `lock` quanh `TryRead`: channel đã thread-safe, không sửa giả định rằng tín hiệu tương đương reservation.

## Nguồn tham khảo
- [Microsoft: ChannelReader.WaitToReadAsync](https://learn.microsoft.com/en-us/dotnet/api/system.threading.channels.channelreader-1.waittoreadasync)
- [Microsoft: Channels](https://learn.microsoft.com/en-us/dotnet/core/extensions/channels)
