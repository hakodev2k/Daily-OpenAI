# Reference Solution — chỉ mở sau khi tự sửa

## 1. Symptoms
Một fragment `CONVEYOR|4` được quan sát nhiều lần dù producer chỉ gửi một lần. `READ_SPIN_GUARD` dừng vòng lặp trước khi phần cuối đến.

## 2. Evidence
`reproduce.ps1` xuất `REPRODUCED: READ_SPIN_GUARD: 4 incomplete observations without a producer write`. Không có newline trong fragment đầu.

## 3. Root cause
`PipeReader.AdvanceTo(consumed, examined)` có hai cursor độc lập. Nhánh chưa có delimiter dùng `AdvanceTo(buffer.Start, buffer.Start)`, báo rằng chưa byte nào được examined. `ReadAsync` tiếp theo có thể trả ngay chính buffer đó, tạo busy loop.

## 4. Why the fix works
Giữ `consumed = buffer.Start` để bảo toàn prefix, nhưng dùng `examined = buffer.End` để cho reader biết toàn bộ buffer hiện có đã được xem. `ReadAsync` chờ byte mới. Khi gặp newline, consume qua delimiter để tránh trả frame trùng. Xem [FrameReader.cs](FrameReader.cs).

## 5. How to verify
Áp dụng sửa vào `starter/FrameReader.cs`, không sửa `Program.cs`, chạy `./verify.ps1`. Reader phải đợi fragment thứ hai, trả `CONVEYOR|42` và chỉ một incomplete observation.

## 6. Alternative fixes
`StreamReader.ReadLineAsync` có thể phù hợp nếu chỉ cần text protocol và không cần kiểm soát pipeline buffers. Với binary protocol, viết parser `ReadOnlySequence<byte>` nhưng vẫn cần quản lý hai cursor.

## 7. Wrong / tempting fixes
- Thêm `Task.Delay` chỉ giảm CPU tạm thời, không sửa hợp đồng đọc.
- Consume `buffer.End` khi chưa đủ frame làm mất prefix.
- Tăng worker hoặc CPU không sửa nguyên nhân.
- Bỏ guard chỉ che triệu chứng.

## 8. Production implications
TCP không bảo toàn ranh giới bản tin. Phải có giới hạn frame size, xử lý EOF/cancellation và đo read-loop progress. Không log payload chứa dữ liệu nhạy cảm.

## 9. Trade-offs
`System.IO.Pipelines` giảm copy và hỗ trợ streaming hiệu quả, đổi lại yêu cầu invariants rõ ràng; `StreamReader` đơn giản hơn cho protocol text nhỏ.

## 10. Senior engineer should notice
Mỗi vòng parser phải consume dữ liệu, examine dữ liệu mới, hoặc chờ producer. Điều kiện progress phải được chứng minh cho mọi nhánh, kể cả oversized frame và EOF.
