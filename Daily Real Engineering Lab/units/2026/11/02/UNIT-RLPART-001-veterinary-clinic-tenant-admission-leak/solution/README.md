# Reference Solution — SPOILER

Chỉ xem sau khi reproduce, tự sửa và verify.

## 1. Symptoms
Beta nhận 429 ngay sau hai request Alpha; Alpha vẫn được nhận thêm trên endpoint khác dù hết quota.

## 2. Evidence
A1/A2/B1 và A3/B2 chỉ ra admission bị ghép theo dimension khác clinic. Header A3 giả Beta để kiểm tra trust boundary. CPU và downstream không liên quan.

## 3. Root cause
Starter dùng `context.Request.Path.Value` làm partition key. Các clinic cùng endpoint chia quota; một clinic đổi endpoint được quota mới.

## 4. Why the fix works
[Reference code](AdmissionPolicy.cs) lấy `clinic_id` từ authenticated principal. Alpha và Beta có partition độc lập, Alpha dùng chung quota qua hai endpoint. Không tin `X-Clinic-ID`.

## 5. How to verify
```powershell
Copy-Item ./solution/AdmissionPolicy.cs ./starter/AdmissionPolicy.cs -Force
./verify.ps1
```
`verify.ps1` kiểm tra starter đã chỉnh, không kiểm tra reference file. Kỳ vọng A1=200 A2=200 B1=200 A3=429 B2=200 B3=429. Sau fix `reproduce.ps1` phải fail vì triệu chứng đã biến mất.

## 6. Alternative fixes
ASP.NET Core `AddRateLimiter` named policy theo claim là cách tích hợp thật. Gateway per-tenant quota phù hợp khi cần quota toàn cluster, không bắt buộc cho local instance.

## 7. Wrong / tempting fixes
Tăng PermitLimit che triệu chứng nhưng sai contract. Partition theo header cho phép spoof. Partition theo `clinic+path` vẫn bypass quota khi đổi endpoint. Retry 429 không sửa identity.

## 8. Production implications
Authentication phải chạy trước limiter dựa trên claim; missing identity xử lý 401/403 upstream. Metrics 429 theo clinic đã chuẩn hóa, không theo request ID. Đánh giá cardinality và nhiều replica.

## 9. Trade-offs
Fixed window đơn giản nhưng có burst ở biên. Sliding window/token bucket mượt hơn nhưng tăng cấu hình. Local limiter rẻ nhưng không có global guarantee.

## 10. Senior engineering judgment
Bắt đầu từ contract, trust boundary và evidence đối chứng; phân biệt per-instance với distributed enforcement trước khi đề xuất Redis/gateway.
