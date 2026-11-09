# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms và evidence
Fixture offset=0 chạy đúng; các fixture có header/prefix trước payload cho nhiệt độ sai. Log expected/actual tách biệt với lỗi thiếu byte.

## Root cause
`ArraySegment<byte>` biểu diễn cửa sổ `Offset/Count` trên backing array. Starter đọc `payload.Array.AsSpan(0,2)` nên vô tình đọc hai byte đầu mảng, bỏ qua offset của payload.

## Fix và vì sao đúng
Thay dòng return trong `starter/Program.cs` bằng:
```csharp
return BinaryPrimitives.ReadInt16LittleEndian(payload.AsSpan().Slice(0, sizeof(short)));
```
`payload.AsSpan()` tôn trọng cả offset và count. Kiểm tra `Count >= 2` bảo vệ boundary trước khi đọc.

## Verify
Chạy `./verify.ps1` trên starter đã sửa. Cần cả 4 fixture pass và short-window bị từ chối. Có thể chạy `dotnet run --project starter/SensorGateway.csproj` để xem log.

## Alternative fixes
Dùng `payload.Array.AsSpan(payload.Offset, sizeof(short))` sau khi kiểm tra null và count cũng đúng, nhưng dễ bỏ quên offset khi refactor. Có thể đổi API sang `ReadOnlySpan<byte>` để thể hiện rõ window ownership.

## Wrong / tempting fixes
- Copy toàn bộ receive buffer sang mảng mới: không sửa hợp đồng chọn vùng và tăng allocation.
- Đổi endianness: làm sai dữ liệu hợp lệ; byte order không giải thích sự phụ thuộc vào prefix.
- Chỉ kiểm tra `Array.Length >= 2`: không chứng minh payload window đủ byte.

## Production implications và trade-offs
Không bao giờ diễn giải backing array như payload khi frame parser đã cung cấp slice. Nên dùng span boundary, fixture có prefix/suffix, và kiểm tra truncated frame; tránh log raw sensor payload nếu chứa dữ liệu nhạy cảm. Một Senior engineer phải phân biệt byte order, framing và buffer ownership bằng bằng chứng thay vì sửa theo cảm giác.
