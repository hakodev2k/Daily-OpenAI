# Reference Solution — Spoiler

Chỉ xem sau khi reproduce, ghi hypothesis và tự sửa.

## 1. Symptoms

Gateway trả `transport-length-mismatch` cho cả ba frame hợp lệ; `sentBytes` lớn hơn `declaredBytes`. Nhiệt độ và sensor ID đầu vào không bất thường.

## 2. Evidence

Trong starter, `MemoryStream` được khởi tạo với capacity 128/256/512, nhưng chỉ ghi một frame ngắn. `DeclaredLength` phản ánh số byte đã ghi, còn `Payload.Length` là kích thước backing array được chuyển cho transport.

## 3. Root cause

`MemoryStream.GetBuffer()` phơi bày **toàn bộ vùng lưu trữ được cấp phát**, không cam kết mảng trả về chỉ có `Length` byte dữ liệu. Transport gửi nguyên mảng, làm sai hợp đồng kích thước frame; phần đuôi chưa ghi có thể bị gửi đi.

## 4. Why the fix works

Trong [FrameExporter.cs](FrameExporter.cs), `ToArray()` trả một mảng mới chỉ chứa byte đã ghi. `Payload.Length` bằng `DeclaredLength`, receiver đọc đúng marker, sensor ID, temperature và không còn trailing bytes. Reference file có thể copy đè lên `starter/FrameExporter.cs` **sau khi** bạn đã tự thử.

```powershell
Copy-Item .\solution\FrameExporter.cs .\starter\FrameExporter.cs -Force
.\scripts\verify.ps1
```

## 5. How to verify

Chạy `verify.ps1` trên **starter đã sửa**. Ba case hợp lệ phải ACCEPT, negative truncated frame phải REJECT. `reproduce.ps1` được thiết kế để xác nhận lỗi ban đầu, nên không còn pass sau fix.

## 6. Alternative fixes

- `TryGetBuffer(out ArraySegment<byte> segment)` và truyền `ReadOnlyMemory<byte>(segment.Array!, segment.Offset, segment.Count)` qua API transport nhận slice: giảm copy nhưng yêu cầu giữ backing storage sống và bất biến cho đến khi send hoàn tất. Không được chỉ truyền `segment.Array`.
- Viết trực tiếp vào `IBufferWriter<byte>`/pipeline với lifetime contract rõ ràng khi throughput và allocation thực sự là bottleneck.

## 7. Wrong / tempting fixes

- Tăng `DeclaredLength` cho bằng capacity: hợp thức hóa byte đuôi không thuộc frame, làm sai wire protocol.
- Bỏ kiểm tra length ở gateway: che giấu lỗi và có nguy cơ chấp nhận payload không hợp lệ.
- Chỉ tăng capacity hoặc retry nhiều lần: không tác động nguyên nhân.
- Truyền `GetBuffer()` nhưng chỉ log `Length`: metrics đẹp hơn mà bytes thực gửi vẫn sai.

## 8. Production implications

Đo `payload length` và `wire length` riêng, tránh log nội dung frame có dữ liệu nhạy cảm. Nếu chuyển sang zero-copy, kiểm tra bất biến về ownership, concurrent send, pool reuse và mutation sau enqueue.

## 9. Trade-offs

`ToArray()` đơn giản, an toàn ownership và phù hợp frame nhỏ; tốn thêm allocation/copy O(n). Slice-based transport tiết kiệm copy nhưng đòi hỏi thay đổi API và quản lý lifetime; không tối ưu trước khi đo throughput/GC.

## 10. Senior engineer should notice

Một trường `DeclaredLength` đúng không bảo đảm bytes được gửi đúng. Boundary giữa serializer và transport phải quy định rõ byte range, ownership và thời điểm dữ liệu không còn được phép thay đổi. Negative regression test phải giữ nguyên.
