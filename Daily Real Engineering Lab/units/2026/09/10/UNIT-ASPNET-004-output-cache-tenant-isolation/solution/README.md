# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Request đầu tiên cho `tenant-a` trả `Premium`. Request kế tiếp gửi `X-Tenant-Id: tenant-b` nhưng vẫn có thể nhận payload của `tenant-a`.

## 2. Evidence

Log endpoint chỉ xuất hiện cho request đầu tiên. Điều này cho thấy request thứ hai có thể được phục vụ trước khi endpoint handler chạy. Khi restart process và đổi tenant gọi đầu tiên, response bị reuse cũng đổi theo.

## 3. Root cause

Output cache policy đang cache theo request dimensions mặc định nhưng endpoint lại tạo output phụ thuộc vào `X-Tenant-Id`. Header này chưa được đưa vào cache variation rules, nên hai tenant có thể map tới cùng cached response.

## 4. Why the fix works

Thêm:

```csharp
.SetVaryByHeader("X-Tenant-Id")
```

vào policy khiến cache key phân biệt response theo tenant header. `tenant-a` và `tenant-b` không còn dùng chung cached representation.

## 5. How to verify

Áp dụng fix tương đương trong `starter/`, sau đó chạy:

```powershell
./verify.ps1
```

Kết quả mong đợi:

```text
VERIFY=passed
```

## 6. Alternative fixes

- Tắt output caching cho endpoint nếu dữ liệu quá nhạy cảm, biến động mạnh hoặc khó xác định đầy đủ các dimensions tạo nên representation.
- Chuyển tenant identity thành một route/query dimension rõ ràng nếu contract API thực sự phù hợp với mô hình đó, rồi vary cache theo dimension tương ứng.
- Dùng custom `VaryByValue` khi tenant identity được resolve từ context khác thay vì request header trực tiếp.

## 7. Wrong or misleading fixes

- **Clear cache mỗi request**: che symptom nhưng triệt tiêu lợi ích của caching và không sửa cache-key contract.
- **Đổi TTL xuống rất ngắn**: chỉ giảm cửa sổ lỗi, vẫn có thể leak response giữa tenant trong thời gian cache còn hiệu lực.
- **Clone response object trong endpoint**: không giúp nếu request được phục vụ từ output cache trước khi endpoint chạy.
- **Thêm lock quanh endpoint**: serialization không làm hai representation khác nhau tự có cache key khác nhau.

## 8. Production implications

Cache key phải chứa mọi input có ảnh hưởng tới representation: tenant, locale, selected headers, route/query dimensions hoặc authorization context khi caching được cho phép. Với multi-tenant systems, thiếu một dimension có thể biến lỗi performance optimization thành data-isolation incident.

## 9. Trade-offs

Vary theo tenant tăng số lượng cache entries và giảm hit ratio so với một global entry, nhưng đó là chi phí cần thiết nếu response thật sự khác nhau theo tenant. Nếu cardinality tenant rất lớn, cần đánh giá memory pressure, TTL, eviction và liệu output caching có còn phù hợp hay không.

## 10. What a Senior engineer should notice

Không chỉ hỏi “cache có hoạt động không”; phải hỏi “cache key đại diện cho contract nào?”. Khi representation phụ thuộc vào request context, cache correctness là bài toán identity và isolation trước khi là bài toán hit ratio.

## Authoritative references

- ASP.NET Core output caching: https://learn.microsoft.com/aspnet/core/performance/caching/output?view=aspnetcore-8.0
- `OutputCachePolicyBuilder.SetVaryByHeader`: https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.outputcaching.outputcachepolicybuilder.setvarybyheader?view=aspnetcore-8.0
