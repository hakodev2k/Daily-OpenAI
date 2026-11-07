# Reference Solution — chỉ xem sau khi đã tự thử

## 1. Symptoms
Application hoạt động ở site root nhưng generated links thiếu prefix khi được mount dưới application path, dẫn tới 404.

## 2. Evidence
Root case `/reports/42` đúng; sub-path case vẫn trả `/reports/42` thay vì giữ `/ops`.

## 3. Root cause
Code coi route bắt đầu bằng `/` là URL hoàn chỉnh trong phạm vi site và bỏ qua application base path. Hosting dưới IIS application/sub-application làm base path trở thành một phần của public URL contract.

## 4. Why the fix works
Kết hợp normalized application base path với resource route làm URL phản ánh đúng hosting topology, đồng thời base rỗng vẫn giữ behavior cũ.

## 5. How to verify
Chạy `./verify.ps1`. Script kiểm tra root, một-level sub-path và nested sub-path trên chính `starter/` mà learner đã sửa.

## 6. Alternative fixes
Trong ASP.NET Core thực tế, ưu tiên framework URL generation (`LinkGenerator`, `IUrlHelper`, Razor tag helpers) khi phù hợp thay vì tự nối chuỗi URL. Nếu reverse proxy thay đổi prefix, cấu hình middleware/proxy contract phải phản ánh topology thực tế.

## 7. Wrong or misleading fixes
- Hard-code `/ops`: chỉ chạy ở một environment.
- Sửa IIS để luôn deploy ở root: né constraint thay vì làm code portable.
- Chỉ sửa static HTML links: bỏ sót URL được sinh ở API, redirect hoặc email.

## 8. Production implications
Sai base path có thể phá links, redirects, callback URLs và assets chỉ ở một topology cụ thể, vì vậy deployment topology cần có integration coverage.

## 9. Trade-offs
Framework URL generation giảm lỗi thủ công nhưng cần request/route context. Pure helper đơn giản và test nhanh nhưng caller phải cung cấp base path chính xác.

## 10. What a Senior engineer should notice
URL correctness là contract giữa application và hosting layer. Khi behavior thay đổi theo topology, cần kiểm tra boundary metadata trước khi vá từng endpoint.