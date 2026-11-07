# Reference Solution — SPOILER
Chỉ đọc sau khi reproduce, ghi hypothesis và thử sửa starter.

## 1. Symptoms
Submit nhãn trả 404 trong khi status cùng host trả 200. BaseAddress chứa API prefix.

## 2. Evidence
BaseAddress `https://partner.example.test/carrier/labels/v2/` nhưng fake partner quan sát POST `/labels/submit` → 404, GET `/carrier/labels/v2/labels/status/SHIP-42` → 200. Fixture không dùng network thật; JSON hợp lệ, nên DNS, TLS, authentication và server downtime không giải thích được khác biệt trong lab.

## 3. Root cause
Quy tắc URI resolution của `HttpClient`: reference bắt đầu bằng `/` là absolute-path reference, thay thế toàn bộ path trong `BaseAddress`. Đối tác cần `/carrier/labels/v2/labels/submit` nhưng nhận `/labels/submit`.

## 4. Why the fix works
Thay `PostAsJsonAsync("/labels/submit", ...)` bằng `PostAsJsonAsync("labels/submit", ...)`. BaseAddress kết thúc bằng `/`, nên relative path không có leading slash được ghép dưới API prefix. Status endpoint vốn đúng không đổi.

## 5. How to verify
```powershell
./verify.ps1
dotnet run --project ./solution/RouteLab.csproj -- --verify
```
Lệnh đầu kiểm tra **starter đã sửa**, lệnh sau self-check của reference. Cả hai phải PASS ở hai prefix.

## 6. Alternative fixes
Có thể xây URI qua bộ cấu hình base-path chuẩn hóa và contract tests. URI absolute cho từng endpoint chỉ hợp lý nếu ownership của environment-specific prefix rõ ràng.

## 7. Wrong / tempting fixes
- Retry 404: chỉ tăng traffic.
- Tăng timeout: 404 trả nhanh, không giải quyết routing.
- Hardcode full URL: có thể qua một môi trường nhưng hỏng prefix thứ hai.
- Xóa prefix khỏi BaseAddress: làm status endpoint hỏng.
- Bỏ qua 404: đánh mất nhãn vận chuyển thật.

## 8. Production implications
Log method, resolved host, path và status theo chính sách bảo mật; không log token hoặc PII. Contract tests nên dùng ít nhất hai base paths.

## 9. Trade-offs
URI relative giữ prefix linh hoạt nhưng đòi hỏi quy ước dấu slash rõ ràng. URI absolute có thể rõ hơn cho endpoint độc lập nhưng phân tán cấu hình môi trường.

## 10. Senior engineer should notice
404 không đủ để kết luận resource không tồn tại: phải đối chiếu resolved request URI với routing contract. Xác định rõ base URI có path hay chỉ origin; tránh thêm retry policy trước khi kiểm tra evidence.
