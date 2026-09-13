# Reference Solution — chỉ xem sau khi đã thử

## 1. Symptoms

`Build` trả payload đúng ngay lập tức, nhưng một operation sau có thể làm payload cũ đổi nội dung.

## 2. Evidence

Hai `ReadOnlyMemory<byte>` có thể trỏ tới cùng backing array. Operation thứ hai ghi bytes mới vào storage đã được operation trước trả về cho reusable pool.

## 3. Root cause

`PayloadBuilder` trả một view tới borrowed buffer rồi kết thúc ownership của buffer ngay trong cùng method. Caller vẫn giữ reference tới storage mà producer đã tuyên bố là có thể tái sử dụng.

`ReadOnlyMemory<byte>` không cung cấp ownership và không bảo đảm backing storage không bị thay đổi bởi owner khác.

## 4. Why the fix works

Với contract hiện tại, cách đơn giản là copy phần payload hợp lệ sang một array owned trước khi trả borrowed buffer:

```csharp
public ReadOnlyMemory<byte> Build(string value)
{
    var buffer = pool.Rent();
    var written = Encoding.UTF8.GetBytes(value, buffer);

    var owned = buffer.AsSpan(0, written).ToArray();
    pool.Return(buffer);
    return owned;
}
```

Caller nhận storage có lifetime độc lập với pool nên operation sau không thể mutate payload cũ qua việc reuse buffer.

## 5. How to verify

Sửa `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Expected: exit code `0`, `First afterwards` vẫn là `ORDER-1001`.

## 6. Alternative fixes

Nếu mục tiêu thật sự là zero/low allocation, API có thể trả một object sở hữu lease (`IMemoryOwner<byte>` hoặc abstraction tương tự) và yêu cầu caller dispose sau khi consumer hoàn tất. Đây là contract phức tạp hơn nhưng cho phép ownership rõ ràng mà không copy ngay tại boundary.

Một lựa chọn khác là để producer giữ lease cho đến khi downstream I/O hoàn tất, thay vì trả memory ra ngoài method.

## 7. Wrong or misleading fixes

- Đổi `ReadOnlyMemory<byte>` thành `Memory<byte>`: thay đổi mutability API, không sửa lifetime.
- Không gọi `Return`: triệu chứng biến mất nhưng phá mục tiêu pooling và có thể giữ tài nguyên lâu dài.
- Delay operation thứ hai: chỉ làm race/lifetime violation khó xuất hiện hơn.
- Clear buffer khi return: có thể làm payload cũ thành zero ngay lập tức; vẫn sai ownership.

## 8. Production implications

Ownership bug với pooled memory thường tạo silent data corruption, khó hơn exception vì dữ liệu có thể đúng trong log/debug rồi đổi trước khi network/database consumer sử dụng.

## 9. Trade-offs

Copy tạo allocation nhưng contract đơn giản và an toàn. Lease ownership giảm copy nhưng tăng lifecycle complexity, yêu cầu dispose discipline và failure-path handling.

## 10. What a Senior engineer should notice

Tối ưu allocation không chỉ là chọn pool. Cần định nghĩa rõ ai sở hữu memory, lifetime kết thúc lúc nào, async boundary nào giữ reference, và API contract có truyền ownership hay chỉ truyền một view. Optimization chỉ hợp lệ khi correctness của lifetime được chứng minh trước.