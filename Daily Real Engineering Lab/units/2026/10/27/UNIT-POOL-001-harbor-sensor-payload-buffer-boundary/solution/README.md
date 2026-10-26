# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms

Gói 5 bytes được gửi dưới dạng Base64 nhưng đối tác giải mã ra 32 bytes. Gói đầy 32 bytes vẫn pass.

## 2. Evidence

FixedSizePool trả array 32 bytes có marker synthetic 0xD7. Stream chỉ chứa declaredLength bytes. Reproduce cho declared=5, decoded=32; verify fail với payload ngắn.

## 3. Root cause

ArrayPool<byte>.Rent(minimumLength) trả ít nhất minimumLength bytes, có thể lớn hơn. Convert.ToBase64String(byte[]) encode toàn bộ array thay vì chỉ phần chứa dữ liệu hợp lệ. Phần dư có thể mang bytes cũ từ lần thuê trước.

## 4. Why the fix works

[PacketPayloadEncoder.cs](PacketPayloadEncoder.cs) dùng overload chỉ rõ offset và length:

~~~csharp
return Convert.ToBase64String(rented, 0, declaredLength);
~~~

ReadExactly bảo đảm đã đọc đủ declaredLength bytes hoặc ném EndOfStreamException. finally luôn hoàn trả rental, kể cả khi exception.

## 5. How to verify

Sửa starter rồi chạy:

~~~powershell
./verify.ps1
~~~

Reference có thể tự kiểm chứng riêng:

~~~powershell
dotnet run --project ./solution/TelemetryRelay.csproj -- --verify
~~~

5/5 case phải pass, returnedOnce=True.

## 6. Alternative fixes

- Dùng overload ReadOnlySpan<byte> giới hạn ở valid slice.
- Cấp phát byte[] vừa kích thước với payload nhỏ nếu benchmark chứng minh pool không có lợi.
- Encode vào destination span khi có lý do hiệu năng và tests đủ mạnh.

## 7. Wrong / tempting fixes

- Padding zero đến 32 bytes vẫn vi phạm wire contract.
- clearArray khi Return diễn ra sau serialize, không chữa dữ liệu outbound.
- Nới length check phía đối tác chỉ che giấu lỗi và tăng rủi ro lộ dữ liệu.
- Test chỉ payload đầy 32 bytes bỏ sót boundary failure.

## 8. Production implications

Giới hạn declaredLength từ nguồn không tin cậy trước Rent; giữ finally; không log raw payload nhạy cảm; theo dõi length mismatch. Fixture 32 bytes chỉ phục vụ lab.

## 9. Trade-offs

ArrayPool giảm allocations nhưng tăng nghĩa vụ quản lý ownership, capacity và valid length. clearArray có chi phí CPU nhưng hữu ích khi xử lý dữ liệu nhạy cảm. Với payload rất nhỏ, đo trước khi tối ưu.

## 10. Senior engineering insight

Requested capacity, rented capacity và valid data length là ba đại lượng khác nhau. Hợp đồng truyền tải chỉ được dựa vào valid data length. Kiểm thử phải bao phủ input ngắn hơn capacity và buffer có dữ liệu dư khác 0.

## Regression

Giữ test 0, 4, 5, 31, 32 bytes cùng assertion về Return. Nếu đổi wire format, nguyên tắc valid-length vẫn giữ nguyên.
