# Reference solution — chỉ đọc sau khi tự verify

**Root cause:** code đo elapsed bằng hiệu hai giá trị wall clock. Đồng hồ UTC có thể bị NTP, VM host hoặc thao tác quản trị điều chỉnh; timestamp phục vụ audit không có bảo đảm tăng đơn điệu. Vì thế 3 giây thực có thể bị ghi -2 giây.

Thay nguồn đo thời lượng trong `TransitSlaMonitor.Measure` bằng monotonic counter. Reference `Program.cs` dùng `MonotonicMilliseconds` do `IClock` cung cấp; ứng dụng thực có thể dùng `Stopwatch.GetTimestamp`/`Stopwatch.GetElapsedTime` hoặc `TimeProvider.GetTimestamp`/`GetElapsedTime`. Giữ `UtcNow` riêng cho mốc sự kiện audit, không dùng hiệu wall-clock làm SLA.

Kiểm tra điều kiện monotonic end >= start và regression khi wall clock tiến bình thường. Với các tiến trình restart, monotonic counter không dùng làm timestamp lưu DB hoặc so thời lượng xuyên host; lưu mốc UTC, event IDs và trạng thái xử lý theo contract riêng.

## Wrong fixes
- `Math.Abs(duration)`: che lỗi; khi clock nhảy tiến 2 giờ sẽ báo SLA 2 giờ giả.
- `Math.Max(0, duration)`: biến 3 giây thành 0, gây sai phân phối latency.
- Bắt buộc NTP tắt: không giải quyết được clock correction hoặc clock virtualization; tăng rủi ro vận hành.
- Chuyển `DateTimeOffset` sang `DateTime`: không biến wall clock thành monotonic.

## Trade-offs
Monotonic duration đúng cho hoạt động trong cùng process/host; UTC timestamp cần cho audit, correlation và lưu trữ. Khi đo qua nhiều process, dùng span duration đo tại mỗi hop và correlation IDs, tránh trừ timestamp của hai máy rồi gọi đó là elapsed chính xác.

## Reference self-check
Sao chép `solution/Program.cs` sang `starter/Program.cs` sau khi lưu bài làm rồi chạy `./scripts/verify.ps1`. Không thay đổi project file.
