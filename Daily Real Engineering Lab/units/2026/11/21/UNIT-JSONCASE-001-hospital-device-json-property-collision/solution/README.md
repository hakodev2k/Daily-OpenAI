# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
Bản tin có cả `TemperatureC` và `temperatureC` có thể dẫn đến cảnh báo sai.

## Evidence
`Samples.All[2]` chứa hai tên trường chỉ khác chữ hoa/thường. Chạy contract checks: hai case nhập bình thường được xử lý, nhưng bản tin mơ hồ và trường bắt buộc bị thiếu không được từ chối trong starter.

## Root cause
`PropertyNameCaseInsensitive = true` cho phép các tên JSON khác nhau ánh xạ vào cùng một property .NET; nếu không có validation, giá trị đến sau có thể ghi đè giá trị trước. JSON cú pháp hợp lệ không đồng nghĩa với hợp đồng dữ liệu an toàn.

## Why the fix works
Duyệt `JsonDocument.EnumerateObject()`, dùng `HashSet<string>(StringComparer.OrdinalIgnoreCase)` phát hiện collision **trước** khi deserialize. Kiểm tra required fields, sau đó giữ tương thích với firmware dùng lowercase.

## How to verify
Chạy `./scripts/verify.ps1` sau khi thay đổi starter. Có thể chạy riêng reference bằng `dotnet run --project solution/DeviceLab.csproj -- --contract-check`.

## Alternative fixes
Bắt buộc canonical casing và từ chối mọi biến thể là chính sách chặt hơn nhưng có thể phá tương thích firmware. Có thể dùng `Utf8JsonReader` streaming để giảm allocation cho payload lớn.

## Wrong / tempting fixes
Sắp xếp key hoặc chọn giá trị đầu tiên chỉ che giấu mâu thuẫn; tắt case-insensitive không tự phát hiện duplicate cùng spelling; chỉ kiểm tra range sau deserialize có thể bỏ lỡ giá trị bị ghi đè.

## Production implications
Trả 400 cho payload mơ hồ; log correlation ID và tên trường, không log dữ liệu bệnh nhân; metric theo firmware, quarantine và cảnh báo khi tỷ lệ reject tăng.

## Trade-offs
Validation thêm một lượt parse và allocation; với telemetry nhạy cảm và payload nhỏ, tính đúng đắn quan trọng hơn overhead. Benchmark trước khi tối ưu.

## Senior engineer should notice
Định nghĩa schema và contract tests theo version firmware; xem xét nullable required semantics, strict number handling, payload size limits, duplicate keys nested, và rollback plan cho firmware lỗi.
