# Reference Solution — SPOILER

**Chỉ đọc sau khi reproduce, ghi hypotheses và tự sửa.** File `MeasurementReader.cs` trong thư mục này là bản tham chiếu; không được `verify.ps1` tự động sử dụng.

## 1. Symptoms

Hai JSON hợp lệ parse thành công nhưng việc publish lỗi `ObjectDisposedException`. JSON sai cú pháp vẫn bị từ chối bằng `JsonException`.

## 2. Evidence

Call chain: `MeasurementPublisher.Publish` gọi `MeasurementReader.ReadMeasurement`; sau khi hàm reader trả về, lệnh truy cập `GetProperty("stationId")` trên element mới phát sinh lỗi. Cần đặt breakpoint ở hai vị trí này và kiểm tra thời điểm giải phóng tài nguyên.

## 3. Root cause

`JsonElement` thường tham chiếu dữ liệu do `JsonDocument` sở hữu. Reader trả về một element lấy từ document, trong khi `using` giải phóng document khi hàm kết thúc. Caller giữ view không còn hợp lệ. Đây là lỗi ownership/lifetime, không phải JSON sai hay lỗi transport.

## 4. Why the fix works

Bản tham chiếu gọi `Clone()` trên subtree trước khi rời phạm vi `using`. Element trả về có dữ liệu độc lập với document gốc; caller đọc an toàn sau khi document gốc đã dispose.

Thay duy nhất `starter/MeasurementReader.cs` bằng logic trong [MeasurementReader.cs](MeasurementReader.cs) **sau khi** đã tự làm lab. Không cần thay đổi `Program.cs`, `MeasurementPublisher.cs` hoặc scripts.

## 5. How to verify

```powershell
./verify.ps1
```

Kiểm tra hai station, giá trị decimal và negative test JSON sai cú pháp. Kết quả dự kiến ở [../expected-results/after.md](../expected-results/after.md). Đây là kiểm tra hành vi của code trong `starter/`, không phải chỉ build solution.

## 6. Alternative fixes

- **DTO projection trong phạm vi document:** deserialize/map trực tiếp sang `PublishedReading` trước khi dispose. Ít phụ thuộc DOM và thể hiện contract rõ, nhưng cần xử lý field validation.
- **Trả về `JsonDocument` cho caller sở hữu:** caller chịu trách nhiệm dispose. Hữu ích khi cần nhiều subtree, nhưng dễ tạo ownership ambiguity và leak nếu thiếu contract.
- **Clone subtree:** thay đổi nhỏ, phù hợp khi API cần trả `JsonElement` và chỉ giữ một subtree.

## 7. Wrong / tempting fixes

- Bỏ `using` mà không quy định ai dispose: có thể che lỗi nhưng tăng lifetime của pooled buffers và rủi ro memory pressure.
- Bắt `ObjectDisposedException` rồi retry HTTP: dữ liệu vẫn mất quyền sở hữu; retry không giải quyết nguyên nhân.
- Đổi `JsonSerializerOptions` hoặc tăng timeout: không liên quan vòng đời DOM.
- Chuyển toàn bộ JSON sang string rồi parse nhiều lần: có thể đúng trong vài trường hợp nhưng thường tốn allocation và CPU không cần thiết.

## 8. Production implications

Phân biệt ownership của `JsonDocument`, `JsonElement`, `Stream` và các borrowed views. Ghi structured exception type và station correlation ID (không log payload nhạy cảm). Không retry vô hạn lỗi lập trình có tính deterministic.

## 9. Trade-offs

`Clone()` tạo dữ liệu độc lập, có chi phí bộ nhớ; DTO projection có thể phù hợp hơn khi contract ổn định. Nếu payload rất lớn, cân nhắc streaming reader với ownership rõ ràng. Đo allocations trước khi tối ưu.

## 10. Senior engineer should notice

Lỗi có thể xuất hiện cách xa vị trí tạo object; stack ở caller không có nghĩa caller là nguồn gốc. Phân biệt *data ownership* với *value-type syntax*: một struct vẫn có thể là view vào tài nguyên bên ngoài. Thiết kế API cần nói rõ thời gian sống và quyền sở hữu của dữ liệu trả về.
