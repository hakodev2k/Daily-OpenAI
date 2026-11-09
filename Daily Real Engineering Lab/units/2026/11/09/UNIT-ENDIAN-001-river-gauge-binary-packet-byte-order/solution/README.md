# Reference Solution — UNIT-ENDIAN-001

**Chỉ xem sau khi tự verify.**

Trường station ID và sequence sử dụng big-endian, nhưng nhiệt độ 16-bit signed dùng little-endian. Decoder ban đầu gọi `ReadInt16BigEndian` cho trường nhiệt độ. Với bytes `D7 00`, giá trị đúng là 215 (21,5°C), nhưng đọc ngược thành -10496 (-1049,6°C). Bytes `C4 FF` là -60 (-6,0°C), không phải -15105.

Trong `starter/RiverFrameDecoder.cs`, thay duy nhất lời gọi đọc nhiệt độ bằng `BinaryPrimitives.ReadInt16LittleEndian(frame.Slice(2, 2))`. File `solution/RiverFrameDecoder.cs` là bản tham khảo đầy đủ; có thể copy nội dung để so sánh, nhưng `verify.ps1` luôn kiểm tra starter.

Không dùng `BitConverter.ToInt16` mà không xác định byte order: kết quả có thể phụ thuộc kiến trúc máy. Không đảo toàn bộ frame vì hai trường còn lại đã đúng theo network byte order. Không lọc nhiệt độ bất thường để che giấu lỗi dữ liệu.

**Trade-offs:** `BinaryPrimitives` explicit, không cấp phát; kiểm tra đúng độ dài trước khi slicing. Nếu protocol version thay đổi, cần versioned parser và fixture theo spec, không đoán endian dựa vào giá trị có vẻ hợp lý.

**Kiểm tra:** normal 215, negative -60, boundary -32768; station ID và sequence không đổi; frame dài 7 byte bị từ chối. Khi đã sửa, `./scripts/verify.ps1` phải exit 0. `reproduce.ps1` chỉ dùng với bản gốc và sẽ báo không còn triệu chứng sau khi sửa.
