# Reference Solution — chỉ xem sau khi tự reproduce và thử sửa

**Cách so sánh:** `solution/FrameDecoder.cs` là implementation tham khảo. `verify.ps1` **không** đọc file solution; để so sánh, tự đối chiếu rồi sửa `starter/FrameDecoder.cs`.

## 1. Triệu chứng
Cùng một frame hợp lệ chạy được với `MemoryStream` nhưng thất bại với `FragmentedStream`, thường báo thiếu header hoặc payload.

## 2. Bằng chứng
`FragmentedStream` cố ý trả về tối đa 2 byte cho mỗi lần đọc. Dữ liệu gốc không mất byte. Baseline xem một lần đọc ngắn là lỗi giao thức.

## 3. Root cause
`Stream.ReadAsync` trả về số byte **hiện có thể đọc** (tối đa độ dài yêu cầu), không cam kết lấp đầy buffer. Một kết quả ngắn khác 0 không phải EOF. Với giao thức length-prefixed, decoder phải tiếp tục đọc cho đến khi đủ N byte hoặc nhận 0 byte (EOF).

## 4. Vì sao fix hoạt động
.NET 8 cung cấp `Stream.ReadExactlyAsync(Memory<byte>, CancellationToken)`: đọc đủ byte hoặc ném `EndOfStreamException` khi EOF thực sự đến sớm. Áp dụng độc lập cho 4 byte header và số byte payload đã được kiểm tra giới hạn.

## 5. Verify
Chạy `./reproduce.ps1` để giữ bằng chứng baseline, sau đó `./verify.ps1` để chạy code đã sửa. Các case: contiguous, fragmented, empty, one-byte chunks, truncated header/body, negative/oversized length, cancellation.

## 6. Alternative fixes
Một vòng `while (offset < length)` với `ReadAsync(buffer.AsMemory(offset), ct)` và kiểm tra `n == 0` cũng đúng, hữu ích khi target framework không hỗ trợ `ReadExactlyAsync`. Cần đảm bảo offset tiến lên và cancellation được truyền ở mọi lần đọc.

## 7. Wrong / tempting fixes
- Tăng kích thước buffer không đảm bảo một lần đọc nhận đủ.
- `Task.Delay` trước khi đọc chỉ làm lỗi khó tái hiện, không sửa hợp đồng Stream.
- Bắt `EndOfStreamException` rồi trả chuỗi rỗng biến frame lỗi thành dữ liệu nghiệp vụ hợp lệ.
- Đọc đến EOF rồi parse toàn bộ không phù hợp kết nối giữ mở với nhiều frame liên tiếp.

## 8. Production implications
Giới hạn kích thước payload **trước** cấp phát; phân biệt EOF với short read; truyền cancellation xuyên suốt; có timeout ở tầng caller nếu nguồn có thể treo mãi; giữ quyền sở hữu và disposal của Stream ở caller. Với protocol có nhiều frame trên cùng kết nối, đọc đúng N byte tránh ăn sang frame tiếp theo.

## 9. Trade-offs
`ReadExactlyAsync` ngắn gọn, dễ review và phù hợp .NET 8. Vòng đọc tự viết cho phép instrumentation chi tiết theo từng fragment nhưng tăng surface bug. Cả hai vẫn có thể chờ vô hạn nếu peer không kết thúc: deadline thuộc hợp đồng I/O bên ngoài.

## 10. Senior engineer cần nhận ra
Test bằng `MemoryStream` thường che khuất semantics của network Stream; thiết kế test double có short reads, EOF, cancellation và giới hạn kích thước để xác minh hành vi giao thức, không phụ thuộc số lần Read.
