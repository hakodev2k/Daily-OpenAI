# Reference Solution — spoiler

## 1. Symptoms
Frame từ nguyên mảng đúng, frame từ cửa sổ có payload sai nhưng header độ dài vẫn đúng. Downstream từ chối checksum hoặc giải mã thành số đo sai.

## 2. Evidence
`offset-two`: expected `10 20 30`, actual `EE DD 10`. `trailing-window`: expected `10 20 30`, actual `AA 10 20`. Cả hai có `Count=3`, chỉ khác vị trí dữ liệu trong backing array.

## 3. Root cause
`ArraySegment<byte>` là view `[Offset, Offset+Count)` trên `Array`. Starter dùng `Buffer.BlockCopy(payload.Array, 0, ...)`, bỏ qua `Offset` và gửi prefix không thuộc payload.

## 4. Why the fix works
Giữ header big-endian và guard; nguồn copy phải bắt đầu tại `payload.Offset`, xem [reference source](TelemetryFrameWriter.cs). Số byte vẫn là `payload.Count`, bảo toàn byte-level protocol và tránh rò dữ liệu ngoài cửa sổ.

## 5. How to verify
Chỉ sửa `starter/TelemetryFrameWriter.cs`; chạy `./verify.ps1` yêu cầu 4/4 PASS, exit 0. `./reproduce.ps1` lúc này exit 1 theo thiết kế. Có thể copy file reference vào starter **sau khi** tự làm để đối chiếu.

## 6. Alternative fixes
`payload.AsSpan().CopyTo(frame.AsSpan(2))` diễn đạt trực tiếp cửa sổ. API nhận `ReadOnlySpan<byte>` cũng giúp tránh nhầm backing array nhưng thay đổi interface và có giới hạn lifetime nếu xử lý async. Copy sang mảng riêng trước khi encode đúng nhưng tăng allocation.

## 7. Wrong or misleading fixes
Dùng `payload.Array.Length` thay `Count` có thể rò thêm dữ liệu. Bỏ checksum ở gateway che giấu lỗi integrity. Chỉ test full-buffer tạo false-green. Đổi byte order không chữa sai payload.

## 8. Production implications
Dữ liệu ngoài cửa sổ có thể thuộc message khác và nhạy cảm. Log correlation ID, hash và length thay vì raw payload; kiểm tra ownership/lifetime nếu backing buffer được tái sử dụng trong pipeline async.

## 9. Trade-offs
Sửa tối thiểu vẫn allocate frame mới, dễ bảo trì. `IBufferWriter<byte>` hoặc `ReadOnlySequence<byte>` có thể giảm copy nhưng tăng độ phức tạp và đòi hỏi contract test cho segmentation và lifetime.

## 10. What a Senior engineer should notice
Không được kết luận codec đúng chỉ vì header đúng. Bất biến gồm `frame.Length == 2+Count`, payload byte-for-byte bằng logical window và không phát ra byte ngoài window. Thử property-based loop cho mọi offset/count của mảng nhỏ, kiểm tra cả empty window và boundary guard.

## Senior extension
Tạo thêm fixture `Offset > 0, Count = 1`, rồi đánh giá zero-copy design trong trường hợp producer tái sử dụng backing buffer.
