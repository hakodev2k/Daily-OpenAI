# Reference Solution — chỉ xem sau khi tự thử

## 1. Symptoms
Capture ghi log thành công nhưng dispatcher ném `ObjectDisposedException` khi đọc payload. Lỗi xảy ra cả với JSON hợp lệ.

## 2. Evidence
`reproduce.ps1` chạy baseline và tìm `CHECK_FAILED ... ObjectDisposedException`. Marker CAPTURE xuất hiện trước DISPATCH. `verify.ps1` ban đầu thất bại vì chạy code người học chưa sửa.

## 3. Root cause
`JsonElement` trả về từ `JsonDocument.RootElement.GetProperty` là view gắn với backing document. `CaptureStage.Capture` trả view ra ngoài `using`; `JsonDocument.Dispose()` xảy ra khi method kết thúc. Dispatcher truy cập view sau đó và gặp `ObjectDisposedException`.

## 4. Why the fix works
Reference thay `root.GetProperty("payload")` bằng `root.GetProperty("payload").Clone()` **trước** khi document bị dispose. Clone sở hữu dữ liệu đủ để tồn tại sau scope gốc, không thay đổi schema hoặc wire output. Thay đổi nằm ở capture-to-dispatch ownership boundary.

## 5. How to verify
Chạy `./scripts/verify.ps1` trên `starter/` sau khi sửa. Để đối chiếu reference, chạy `dotnet run --project solution/JsonLab.csproj -c Release`; phải in `VERIFICATION_PASSED`. Baseline vẫn phải tái hiện lỗi.

## 6. Alternative fixes
- Deserialize payload sang immutable typed DTO tại capture stage: type safety tốt hơn, nhưng phải quản lý schema evolution, nullable và unknown fields.
- Giữ `JsonDocument` sống tới khi dispatcher xử lý xong, kèm ownership/disposal rõ ràng: tránh clone ngay nhưng phức tạp khi queue, retry, fan-out và shutdown.
- Lưu raw JSON string rồi parse ở dispatcher: dễ lưu bền vững nhưng thêm parsing và allocation, cần giới hạn payload size.

## 7. Wrong / misleading fixes
- `try/catch (ObjectDisposedException)` rồi bỏ event: chỉ che mất dữ liệu.
- Thêm `Task.Delay` hoặc giảm concurrency: lỗi lifetime vẫn tồn tại ngay cả khi xử lý tuần tự.
- Bỏ `using` mà không định nghĩa ai dispose: đổi lỗi thành rò rỉ bộ nhớ/tài nguyên.
- Chỉ sửa `solution/` hoặc `baseline/`: không chứng minh learner path đã được sửa.

## 8. Production implications
Một webhook có thể được ACK nhanh nhưng work item thất bại sau đó; cần theo dõi success/failure của dispatcher, dead-letter/retry policy, correlation ID và payload-size limits. Nếu queue bền vững qua process boundary, `JsonElement` trong RAM không phải persistence format.

## 9. Trade-offs
Clone tạo owned DOM và có chi phí allocation theo payload; typed DTO phù hợp contract ổn định, raw JSON phù hợp forwarding nhưng cần validate/size guard. Chọn theo schema volatility, throughput, memory budget và khả năng replay.

## 10. Senior engineer should notice
Đánh giá ownership tại **mọi** async handoff, không chỉ lúc code có `await`. Phân biệt request ACK với durable processing success; thêm regression ở boundary chứ không chỉ test parsing trực tiếp. Đảm bảo failure không bị swallowed và evidence đủ để điều tra production.

## Official references
- [JsonElement.Clone](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement.clone?view=net-8.0)
- [JsonDocument](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocument?view=net-8.0)
- [System.Text.Json DOM](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/use-dom)
