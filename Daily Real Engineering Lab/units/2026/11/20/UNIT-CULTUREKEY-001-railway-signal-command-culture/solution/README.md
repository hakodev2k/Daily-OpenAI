# Reference solution — UNIT-CULTUREKEY-001

## Root cause
`ToUpper(CultureInfo.CurrentCulture)` phụ thuộc locale; trong Turkish, ký tự `i` được uppercase thành `İ`, khiến `signal_open` không còn khớp key ASCII `SIGNAL_OPEN`.

## Fix
Với protocol identifiers chỉ dùng ASCII, chuẩn hóa bằng `ToUpperInvariant()` là thay đổi tối thiểu. Phương án tốt hơn về mô hình dữ liệu là tạo `Dictionary<string,string>(StringComparer.OrdinalIgnoreCase)` và lookup chuỗi nguyên bản, tránh normalization không cần thiết.

## Verification
`./verify.ps1` phải pass sau khi learner sửa starter. `dotnet run --project solution/SignalDispatch.csproj -- --verify` kiểm tra reference độc lập.

## Wrong fixes
- Đổi toàn bộ server về en-US: che giấu bug, không sửa hợp đồng giao thức.
- `ToLower(CurrentCulture)`: vẫn phụ thuộc locale.
- Bỏ case-insensitive: có thể phá tương thích thiết bị gửi chữ thường.

## Trade-offs
Ordinal comparison phù hợp identifiers, không dùng để so sánh tên người hoặc văn bản hiển thị theo ngôn ngữ tự nhiên.
