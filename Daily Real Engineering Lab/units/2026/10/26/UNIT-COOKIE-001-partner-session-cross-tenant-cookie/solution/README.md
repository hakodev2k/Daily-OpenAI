# Reference Solution — SPOILER

Chỉ xem sau khi chạy `reproduce.ps1`, ghi giả thuyết và tự sửa `starter/TenantGateway.cs`.

## 1. Triệu chứng

Alpha đăng nhập thành công; Beta chưa đăng nhập lại gửi request mang trạng thái session Alpha, nhận 403 thay vì 401. Sau Beta đăng nhập, Alpha có thể bị 403.

## 2. Bằng chứng

- Alpha quote: 200, marker `alpha`.
- Beta trước login: 403, marker `alpha` (đáng ra không có session).
- Beta sau login: 200; Alpha sau đó: 403, marker `beta`.
- Không có lỗi routing, timeout hoặc CPU pressure; triệu chứng phụ thuộc thứ tự request trong một process.

## 3. Root cause

`TenantGateway` giữ một `HttpClientHandler` có `UseCookies=true` và một `CookieContainer` cho **mọi tenant**. Cookie container quản lý cookie theo domain/path, **không** tự hiểu tenant identity của ứng dụng. Khi các tenant dùng cùng partner origin và cookie name, session mới ghi đè session cũ; outbound request của tenant khác tự động nhận cookie không thuộc mình.

## 4. Vì sao fix hoạt động

File `solution/TenantGateway.cs` tạo client + handler + cookie jar riêng cho mỗi tenant, dùng `ConcurrentDictionary<string, Lazy<HttpClient>>` để chỉ khởi tạo client cần thiết. Mỗi tenant vẫn reuse connection/session trong phạm vi của mình, nhưng cookie không vượt ranh giới tenant. Copy file tham khảo vào `starter/TenantGateway.cs` **chỉ để so sánh sau khi tự làm**.

## 5. Verification

```powershell
Copy-Item ./solution/TenantGateway.cs ./starter/TenantGateway.cs
pwsh -File ./verify.ps1
```

Kết quả: Alpha 200; Beta chưa login 401; Beta login rồi 200; Alpha vẫn 200. `reproduce.ps1` sau fix sẽ trả non-zero vì không còn bug.

## 6. Alternatives

- Nếu partner cho phép bearer token / stateless authentication: `UseCookies=false`, cung cấp credential theo **từng request** từ kho identity an toàn. Tránh tự quản cookie khi không cần.
- Nếu bắt buộc cookie-based session: dùng tenant-scoped handler/container và chính sách TTL, eviction, refresh; không tạo vô hạn HttpClient cho tenant không hoạt động.
- `IHttpClientFactory` tái sử dụng handler: cần hiểu handler pooling và cookie sharing; factory mặc định không tự cô lập cookie theo tenant.

## 7. Wrong / tempting fixes

- Thêm lock cho mọi outbound call: serialize request nhưng vẫn dùng chung cookie jar, lỗi logic còn nguyên.
- Xóa cookie trước mỗi request: phá session hợp lệ và gây login churn; không bảo đảm isolation nếu request chạy song song.
- Tạo `HttpClient` mới nhưng dùng lại **cùng** handler/container: không giải quyết ranh giới cookie.
- Chỉ retry 403: che giấu lỗi và tăng tải partner.

## 8. Production implications

Cookie thật là credential. Không log cookie, không gửi marker ra public endpoint; test này chỉ dùng `alpha`/`beta` synthetic. Partition theo **identity được xác thực**, không theo user input tự khai. Tính đến tenant offboarding, key rotation, cookie expiry, giới hạn số tenant active và shutdown/drain an toàn.

## 9. Trade-offs

Tenant-scoped clients tăng socket/handler count theo số tenant, nên cần bounded lifecycle hoặc cơ chế session khác ở scale lớn. Stateless per-request auth đơn giản hơn khi partner hỗ trợ. Isolation và correctness quan trọng hơn việc tối đa hóa pooling giữa các security principal.

## 10. Senior engineer cần nhận ra

Phân biệt pooling tài nguyên mạng với pooling **security state**. Mọi stateful handler/cookie jar phải có ownership boundary được định nghĩa rõ; test một tenant không đủ để chứng minh multi-tenant isolation. Dùng ma trận A→B→A, không chỉ happy path A→A.
