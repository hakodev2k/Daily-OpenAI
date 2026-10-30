# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Triệu chứng

Kelp chưa đăng nhập nhận `200 coral`, sau khi Kelp đăng nhập Coral lại nhận `200 kelp`. Đây là vi phạm cô lập tenant, không chỉ là lỗi hiển thị.

## 2. Bằng chứng

Harness sử dụng cùng một host loopback và thực hiện sáu thao tác tuần tự. Server chỉ xác định profile qua cookie `partner_session`; request Kelp chưa đăng nhập không được phép có cookie của Coral.

## 3. Root cause

`PartnerGateway` giữ một `HttpClientHandler` cùng `CookieContainer` cho mọi tenant. Khi `UseCookies=true`, handler tự thêm cookie đã lưu vào request phù hợp theo domain/path, bất kể tham số `tenant` của business method. Login Kelp thay thế cookie Coral trong cùng jar. Đổi tên tenant hay tạo request mới không tạo ranh giới xác thực.

## 4. Vì sao fix hoạt động

[PartnerGateway.cs](PartnerGateway.cs) tạo một cookie jar và handler riêng cho từng tenant. Hai tenant không chia sẻ trạng thái session. Copy nội dung file này đè lên `starter/PartnerGateway.cs` rồi chạy `./verify.ps1`. Trong demo, từ điển được bảo vệ bằng `lock` để tránh tạo trùng client trong thao tác song song.

## 5. Verify

Kiểm tra Kelp trước login là `401`, Coral sau Kelp login vẫn là `200 coral`, đồng thời hai lần đọc đúng của từng tenant không regress. `verify.ps1` chạy **starter do người học sửa**, không chạy solution.

## 6. Alternative fixes

- Tốt hơn khi partner hỗ trợ: bỏ cookie ambient, dùng token riêng cho mỗi request; token phải được cấp và kiểm tra đúng tenant, không dùng shared default header.
- Với cookie bắt buộc: quản lý một session/handler riêng theo tenant hoặc theo user session, có giới hạn bộ nhớ, TTL, thu hồi, khóa đồng thời và cơ chế logout.
- Có thể dùng client ngắn hạn riêng cho từng session trong bài toán tải thấp; phải cân nhắc connection churn, socket và DNS.

## 7. Wrong / tempting fixes

- Thêm `tenant` vào log hoặc query string mà vẫn giữ cookie jar chung: không sửa ranh giới xác thực.
- Gọi `new HttpRequestMessage` cho mỗi lần đọc: request mới vẫn đi qua handler chung.
- Chỉ xóa cookie trước mỗi request: dễ race khi có nhiều request đồng thời và có thể làm hỏng phiên hợp lệ.
- Tăng số instance ứng dụng: không bảo đảm cô lập trong từng instance.

## 8. Production implications

`IHttpClientFactory` thường pool handler; cookie state trong pooled handler có thể gây chia sẻ không mong muốn. Nếu dùng cookie authentication với partner, cần thiết kế handler/session ownership rõ ràng; không mặc định dùng factory pooling như cơ chế phân vùng tenant. Với production, tránh từ điển tenant không giới hạn như demo; cần eviction, lifecycle, bảo vệ concurrent dispose, quản lý credential và kiểm thử đa luồng.

## 9. Trade-offs

Handler riêng giúp cô lập cookie nhưng tăng tài nguyên và quản lý lifecycle. Stateless token per request đơn giản hóa boundary khi partner hỗ trợ, song yêu cầu token issuance/rotation an toàn. Chọn theo hợp đồng partner, mức tải, security boundary và chi phí vận hành.

## 10. Senior engineer nên nhận ra

Kiểm tra nơi lưu identity state **thực sự** thay vì tin vào chữ ký method. Bổ sung regression test cross-tenant trước và sau login; đánh giá pooling, retry, redirect, logout, memory growth và việc ghi log không tiết lộ dữ liệu nhạy cảm.

Tài liệu chính thức: [HttpClientHandler.CookieContainer](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler.cookiecontainer) · [HTTP client guidelines](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines).
