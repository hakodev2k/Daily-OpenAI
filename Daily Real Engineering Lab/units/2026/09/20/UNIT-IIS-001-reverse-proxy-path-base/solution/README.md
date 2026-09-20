# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Request tới `/benefits/account/profile` hoạt động qua proxy, nhưng application sinh navigation `/account/settings`; browser vì vậy rời khỏi mount `/benefits` và production trả 404.

## 2. Evidence
Public request chứa `/benefits`, trong khi upstream chỉ thấy `/account/profile`. Generated URL bắt đầu từ root và không chứa external application prefix.

## 3. Root cause
Deployment có hai path identities nhưng application chỉ biết route identity phía upstream. Reverse proxy đã tạo một external path base mà URL-generation boundary không được cấu hình để bảo toàn.

## 4. Why the fix works
Khi application coi `/benefits` là `PathBase` (hoặc proxy/application phối hợp truyền equivalent forwarding information), link generation kết hợp application base với route path. Trong simulator, sửa `BuildNavigation` để public URL được tạo từ explicit mount boundary thay vì giả định application ở `/`.

Ví dụ tối thiểu cho simulator:
```csharp
static string BuildNavigation(string currentPath)
{
    _ = currentPath;
    return "/benefits/account/settings";
}
```
Trong application thật, ưu tiên cấu hình routing/path-base và URL generation tập trung thay vì hard-code prefix trong từng link.

## 5. How to verify
Chạy `./verify.ps1`. Sau đó kiểm tra cả direct/local topology và proxied topology trong integration/E2E test nếu hệ thống thật hỗ trợ cả hai.

## 6. Alternative fixes
IIS/reverse proxy có thể preserve prefix khi forwarding, hoặc application có thể dùng `UsePathBase`/hosting configuration phù hợp. Chọn một ownership model rõ ràng giữa proxy và app.

## 7. Wrong or misleading fixes
Hard-code `/benefits` ở mọi controller/view chỉ che topology và dễ vỡ khi mount path đổi. Rewrite riêng từng redirect xử lý triệu chứng nhưng bỏ sót static links/callback URLs. Tắt validation/404 handling không sửa URL identity.

## 8. Production implications
Sai path-base có thể ảnh hưởng redirects, generated links, OpenAPI endpoints, auth callback URLs, static assets và health endpoints. Deployment contract cần được test ở boundary thật.

## 9. Trade-offs
Proxy-owned rewriting giữ app đơn giản nhưng đòi hỏi proxy rules nhất quán. App-aware path base làm URL generation có context chính xác hơn nhưng deployment configuration trở thành một phần runtime contract.

## 10. What a Senior engineer should notice
Local success không chứng minh routing correctness sau reverse proxy. Cần mô hình hóa external URL identity và upstream route identity như hai boundary khác nhau, xác định component nào sở hữu việc chuyển đổi, rồi regression-test topology đó thay vì sửa từng 404.