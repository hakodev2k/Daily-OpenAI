# Reference Solution — chỉ xem sau khi tự reproduce và thử sửa

## 1. Symptoms
Hai event độc lập được ingest đúng thứ tự, nhưng event đầu bị thay đổi trước khi batch drain. `reproduce` kiểm tra sự thay đổi có chủ đích.

## 2. Evidence
`OneSlotBufferPool` tái sử dụng cùng một `byte[]` sau `Return`. Queue giữ `ReadOnlyMemory<byte>` trỏ vào array đó; lần ingest kế tiếp ghi đè byte cũ. Đối chiếu `Receive`, `Return`, `Drain` và `ReferenceEquals` khi debug.

## 3. Root cause
Ownership của vùng nhớ không khớp lifetime: producer trả rented buffer ngay sau khi enqueue một **view**. Consumer decode muộn hơn, khi vùng nhớ đã được tái sử dụng. `ReadOnlyMemory<T>` không phải immutable snapshot.

## 4. Why the fix works
`solution/Program.cs` tạo bản sao chỉ đúng `written` byte trước khi trả rented buffer. Queue sở hữu byte array riêng, nên việc tái sử dụng buffer transport không thể sửa nội dung của event cũ. `finally` vẫn bảo đảm trả buffer khi lỗi.

## 5. Verification
`./verify.ps1` chạy trên **starter do learner sửa**, không chạy solution. Kiểm tra 4 case, số lượng, thứ tự, sensor ID, nội dung và UTF-8; exit 0 khi mọi case PASS. Tự kiểm reference bằng `dotnet run --project solution/TelemetryLab.csproj -- verify` (độc lập với learner verification).

## 6. Alternative fixes
- **Transfer ownership**: queue giữ lease và trả buffer sau khi decode; giảm copy nhưng cần contract dispose/ack rõ ràng, kể cả lỗi và shutdown.
- **Immutable payload tại boundary**: copy như reference, dễ reasoning, tăng allocation theo byte ingest.
- **Decode sớm thành string**: phù hợp nếu consumer chỉ cần text; tăng chi phí decode ở ingress, đổi contract của queue.

## 7. Wrong / tempting fixes
- Dùng `ReadOnlyMemory<byte>` hoặc `AsReadOnly` không biến underlying bytes thành bất biến.
- `lock` quanh `Receive` không bảo vệ lifetime sau khi hàm trả về; lỗi có thể xảy ra ngay cả single-thread.
- Chỉ tăng kích thước pool làm lỗi ít lộ hơn, không sửa ownership.
- Delay `Drain` hoặc tăng timeout không giúp; còn có thể khiến tái sử dụng nhiều hơn.

## 8. Production implications
Với `ArrayPool<byte>.Shared`, tái sử dụng có thể không deterministic; lỗi biểu hiện như payload corruption, tenant data mix-up hoặc telemetry sai. Buffer pooling không được dùng làm cơ chế lưu trữ dài hạn khi lease đã kết thúc. Đặt metrics cho malformed payload, queue age và kích thước payload, nhưng không log dữ liệu nhạy cảm.

## 9. Trade-offs
Copy làm tăng allocation và bandwidth bộ nhớ, đổi lại boundary đơn giản và an toàn. Với payload lớn hoặc throughput cao, ownership-transfer/lease có thể tốt hơn nhưng đòi hỏi kiểm soát lifecycle và backpressure. Benchmark allocation trước khi tối ưu.

## 10. Senior engineer should notice
Phân biệt read-only **access** với ownership và lifetime; xác định producer/consumer contract, cancellation và cleanup; thiết kế regression test tái sử dụng deterministic thay vì dựa vào race ngẫu nhiên.

## Câu hỏi mở rộng
Nếu queue có nhiều consumer, retry, crash hoặc drop message, ai trả lease đúng một lần? Làm sao chống use-after-return mà không tăng copy vô hạn?
