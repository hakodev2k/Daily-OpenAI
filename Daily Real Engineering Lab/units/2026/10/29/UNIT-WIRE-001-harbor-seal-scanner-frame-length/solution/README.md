# Reference Solution — chỉ đọc sau khi tự điều tra

## 1. Symptoms
Gateway từ chối `00 04 21 50 49 4E 47` dù đây là frame hợp lệ: header 2 byte, type 1 byte, payload 4 byte. Trong log, `declared` không trùng `actual=4`.

## 2. Evidence
Wire header `00 04` đại diện cho số 4 theo network byte order. Starter giải mã thành 1024. Với `04 00`, starter lại đọc thành 4 và có thể chấp nhận payload 4 byte dù header đó tuyên bố 1024 theo hợp đồng. Hai phép thử này bác bỏ giả thuyết TCP làm mất byte.

## 3. Root cause
Thiết bị gửi unsigned 16-bit length theo **big-endian**, còn gateway dùng `BinaryPrimitives.ReadUInt16LittleEndian`. Đây là mismatch về wire protocol, không phải thiếu buffer.

## 4. Why the fix works
Trong `starter/Program.cs`, thay đúng phép đọc length bằng:

~~~csharp
int declaredLength = BinaryPrimitives.ReadUInt16BigEndian(packet[..2]);
~~~

Không đổi wire bytes hoặc giới hạn payload. `solution/Program.cs` là bản tham khảo đầy đủ, không phải target của `verify.ps1`.

## 5. How to verify
Sau khi chỉnh `starter/Program.cs`, chạy `./verify.ps1`. Ba vector hợp lệ (`PING`, `ACK`, empty payload) phải pass; ba vector sai (payload thiếu byte, header đảo, header thiếu) phải bị từ chối. Chạy `./reproduce.ps1` sau sửa sẽ không còn tái hiện symptom và có thể exit 1 — điều đó là bình thường.

## 6. Alternative fixes
Có thể tự đọc `(header[0] << 8) | header[1]` nếu cần môi trường không có `BinaryPrimitives`, nhưng API chuẩn diễn đạt rõ ý định và giảm rủi ro sai phép dịch bit. Nếu firmware thực sự có nhiều phiên bản protocol, dùng version negotiation minh bạch, không tự đoán endianness từ dữ liệu.

## 7. Wrong or misleading fixes
- Tăng `MaxPayloadBytes` chỉ che dấu số length bị đọc sai và có thể làm lỏng giới hạn.
- Đảo hai byte payload không sửa lỗi length.
- Retry bản tin không giúp khi cùng input luôn bị diễn giải sai.
- Chấp nhận cả hai endian theo heuristic dễ khiến frame sai được chấp nhận; chỉ phù hợp nếu có version field và đặc tả rõ ràng.

## 8. Production implications
Xác thực toàn bộ frame trước khi cấp phát buffer lớn; giới hạn độ dài, kiểm tra type, phân biệt truncated header với truncated payload. Nếu đọc từ TCP stream, xử lý partial reads riêng (không được giả định một `ReadAsync` trả đủ frame). Theo dõi tỷ lệ reject theo firmware/protocol version mà không log dữ liệu nhạy cảm.

## 9. Trade-offs
Giữ wire contract thống nhất giúp nhiều loại thiết bị tương thích. Version negotiation tăng độ phức tạp nhưng cần thiết khi thay đổi protocol thật sự. Không dùng host endianness làm mặc định của giao thức truyền tải.

## 10. Senior engineer should notice
Cần so sánh byte-level evidence với spec trước khi thay timeout, firmware hay giới hạn payload. Regression phải kiểm tra cả **positive** lẫn **negative** contract; chỉ test PING hợp lệ có thể bỏ sót việc parser chấp nhận frame sai.

Tham khảo chính thức: [BinaryPrimitives.ReadUInt16BigEndian](https://learn.microsoft.com/dotnet/api/system.buffers.binary.binaryprimitives.readuint16bigendian).
