# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms

Application khởi động thành công và có thể bị deployment pipeline xem là healthy. Business operation đầu tiên mới thất bại khi code cố biến `Reports:BaseUrl` rỗng thành absolute `Uri`.

## 2. Evidence

Starter in `APP_STARTED` trước `REQUEST_FAILED:UriFormatException`. Điều đó chứng minh failure boundary đang nằm quá muộn so với lifecycle của deployment.

## 3. Root cause

`ReportOptions` được bind bằng `Configure<T>()` nhưng không có configuration contract được validate trong startup lifecycle. Giá trị sai vẫn tồn tại trong DI container cho tới lúc consumer sử dụng nó.

## 4. Why the fix works

Reference solution đăng ký strongly-typed options bằng `AddOptions<ReportOptions>()`, bind section `Reports`, khai báo validation rule và gọi `ValidateOnStart()`. Khi host start, Options validation chạy trước khi application in marker sẵn sàng. Invalid deployment vì vậy fail fast.

## 5. How to verify

Copy cách sửa tương đương vào `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Verification yêu cầu process fail, không có `APP_STARTED`, không có `REQUEST_FAILED`, và có validation message mô tả contract cấu hình.

## 6. Alternative fixes

- Dùng `IValidateOptions<ReportOptions>` khi rule phức tạp, cần reuse hoặc dependency riêng.
- Dùng DataAnnotations nếu contract phù hợp với attribute-based validation.
- Pipeline preflight validation có thể bổ sung thêm một lớp bảo vệ, nhưng application vẫn nên tự bảo vệ contract runtime của chính nó.

## 7. Wrong / tempting fixes

### Hard-code production URL

Che lỗi deployment và làm code phụ thuộc environment.

### Catch `UriFormatException` rồi dùng URL mặc định

Có thể hợp lệ nếu fallback là một business requirement rõ ràng. Nếu không, nó biến cấu hình sai thành behavior ngầm và có thể gửi dữ liệu tới destination không mong muốn.

### Chỉ thêm health check trả 200

Health endpoint không tự chứng minh configuration contract hợp lệ. Nếu health check không kiểm tra cùng invariant thì deployment vẫn có thể nhận traffic rồi mới hỏng.

### Validate trong request handler

Thông báo lỗi có thể tốt hơn, nhưng failure vẫn xảy ra sau khi instance đã được coi là ready.

## 8. Production implications

Fail-fast configuration validation giúp deployment orchestration, rollback và alerting nhận tín hiệu sớm hơn. Nó cũng giảm blast radius vì invalid instance không nhận business traffic.

## 9. Trade-offs

Validation lúc startup làm startup nghiêm ngặt hơn. Chỉ validate các invariant thực sự cần để instance hoạt động. Với dependency tạm thời ngoài process, đừng biến mọi transient connectivity problem thành configuration validation nếu semantics không yêu cầu như vậy.

## 10. What a Senior engineer should notice

Senior engineer không chỉ sửa `UriFormatException`; họ xác định **failure boundary** đúng. Một invariant có thể biết ngay từ deployment time nên được kiểm tra trước khi instance tuyên bố readiness, đồng thời error phải chỉ rõ key/configuration contract nào sai.