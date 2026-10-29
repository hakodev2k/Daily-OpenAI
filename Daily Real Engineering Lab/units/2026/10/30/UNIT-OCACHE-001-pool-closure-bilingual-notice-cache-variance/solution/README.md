# Reference Solution — chỉ xem sau khi tự sửa

## 1. Symptoms
Cùng endpoint trả HTTP 200 nhưng khách dùng `en-US` nhận thông báo tiếng Việt sau khi khách dùng `vi-VN` truy cập trước. `renderCount` không tăng ở request thứ hai.

## 2. Evidence
Thử đổi thứ tự truy cập trên server mới, giữ URL và body cố định, chỉ thay `Accept-Language`. Kiểm tra `renderCount` để xác nhận endpoint có chạy hay response cũ được phục vụ lại. Không kết luận chỉ từ HTTP status hoặc latency.

## 3. Root cause
Endpoint chọn ngôn ngữ dựa trên header `Accept-Language`, nhưng output-cache policy ban đầu không khai báo header đó là chiều phân biệt cache key. Hai request có ý nghĩa nghiệp vụ khác nhau dùng chung một cached response.

## 4. Why the fix works
`solution/Program.cs` thêm `.SetVaryByHeader("Accept-Language")` vào policy `closure-notices`. Response tiếng Việt và tiếng Anh không còn chia sẻ cùng cache entry; request lặp lại trong cùng header vẫn có thể reuse entry của mình.

## 5. Verification
Chỉ sửa `starter/Program.cs` theo ý tưởng của bạn rồi chạy `./verify.ps1`. Script chạy chính starter, kiểm tra nội dung hai ngôn ngữ, hai lần truy cập mỗi ngôn ngữ và các số `renderCount`. `./reproduce.ps1` phải không còn tái hiện được lỗi.

## 6. Alternative fixes
- Dùng normalized locale như `vi` / `en` làm custom vary value để giới hạn số entries khi nhiều giá trị `Accept-Language` khác nhau cùng ánh xạ về một ngôn ngữ. Cần kiểm thử đúng API và chính sách fallback.
- Không cache endpoint khi tần suất thấp hoặc thông báo thay đổi liên tục; đánh đổi latency và backend load.
- Đưa locale vào route `/vi/...` hoặc `/en/...` nếu API contract và CDN thống nhất, cần migration và compatibility.

## 7. Wrong / tempting fixes
- Chỉ thêm `Vary: Accept-Language` vào HTTP response: header đó giúp downstream HTTP caches, nhưng không thay thế việc khai báo variation cho ASP.NET Core output-cache policy.
- Tắt cache toàn hệ thống: có thể che triệu chứng nhưng phá tối ưu không cần thiết.
- Tăng TTL hoặc restart service: không giải quyết ranh giới biến thể.
- Chỉ sửa logic dịch trong endpoint: request trả từ cache có thể không vào endpoint.

## 8. Production implications
Kiểm tra reverse proxy/CDN cũng tôn trọng variation hoặc dùng locale route rõ ràng. Có thể cần purge entries cũ khi deploy. Theo dõi tỉ lệ hit, số entries, memory và correctness bằng synthetic probes.

## 9. Trade-offs
Raw `Accept-Language` có cardinality cao (thứ tự preference, quality values, region variants). Giải pháp tối thiểu phù hợp lab; production nên cân nhắc normalize locale trước cache key, phân vùng CDN nhất quán và TTL theo tần suất cập nhật thông báo.

## 10. Senior engineer should notice
Không phải mọi response HTTP 200 đều đúng nghiệp vụ. Cache key là một phần của hợp đồng correctness, không chỉ performance. Cần kiểm tra cả response variant, reuse, negative cases và lớp cache ở nhiều hop.

## Source comparison
- [Corrected runnable Program.cs](Program.cs)
- [Original learner-editable starter](../starter/Program.cs)
- [Verification contract](../expected-results/after.md)
