# Reference Solution — chỉ đọc sau khi tự điều tra
Lỗi gốc: callback của hàng đợi giữ reference đến mảng thuê, nhưng producer trả mảng trước khi callback đọc. Pool tái sử dụng cùng vùng nhớ cho `CLEAR`, khiến job `ALERT` phát `CLEAR`. Đây là lỗi ownership/lifetime, không phải lỗi ASCII.

Thay `Enqueue` trong `starter/Program.cs` bằng:
```csharp
public void Enqueue(byte[] buffer, int count)
{
    byte[] owned = buffer.AsSpan(0, count).ToArray();
    _pending.Add(() => Encoding.ASCII.GetString(owned));
}
```
Sau đó chạy `./verify.ps1`. Không chỉnh script verify. Cách này tạo snapshot riêng cho từng job; overhead là một allocation và copy O(n) mỗi payload. Phương án khác là chuyển quyền sở hữu lease cho consumer và chỉ trả pool sau khi xử lý xong; cần thiết kế `IDisposable`/finally, backpressure và xử lý cancellation để tránh leak.

## Wrong fixes
- `Thread.Sleep`: chỉ đổi timing, không xác lập ownership.
- Tăng kích thước pool: giảm xác suất tái sử dụng chứ không sửa hợp đồng vòng đời.
- Dùng `lock` quanh `Enqueue`: không bảo vệ giai đoạn consumer đọc sau đó.
- Dùng `ArrayPool.Shared` thay pool mô phỏng: lỗi vẫn tồn tại, chỉ khó reproduce hơn.

## Root cause và trade-off
Dữ liệu được giữ lại qua callback phải có lifetime dài hơn callback, hoặc phải được copy sang vùng nhớ do callback sở hữu. Với payload nhỏ và SLA an toàn, copy thường dễ chứng minh đúng hơn. Với payload lớn, ownership transfer có thể tiết kiệm allocation nhưng làm tăng complexity và rủi ro leak.

## Senior extension
Thiết kế API `EnqueueAsync(ReadOnlyMemory<byte>, CancellationToken)` với hợp đồng ownership tường minh, queue capacity 100, shutdown drain và kiểm thử concurrent producer. Không nhận `ReadOnlyMemory` làm bằng chứng immutable.
