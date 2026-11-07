# Reference Solution — chỉ xem sau khi tự thử

## 1. Symptoms

Vé hợp lệ đôi khi bị partner từ chối; một số pass thông thường vẫn được chấp nhận. Trạng thái lỗi phụ thuộc nội dung pass, không phụ thuộc đồng hồ hay kết nối mạng.

## 2. Evidence

`SKI+NORTH/27==` khi đi qua request URL nguyên dạng có thể được partner form-style query decoder đọc thành `SKI NORTH/27==`. Chuỗi `CABIN&ZONE=4` bị chia thành hai query parameters; `SNOW#VIP` có phần sau `#` thành URI fragment; `PCT%2B` bị diễn giải thành `PCT+`.

## 3. Root cause

`PartnerClient.BuildValidationUri` ghép một **opaque pass value** trực tiếp vào query URI. Các ký tự reserved không được escape ở cấp query component. URI và partner decoder đều thực hiện đúng contract riêng; client đã xây dựng request sai.

## 4. Why the fix works

Trong `solution/PartnerClient.cs`, `Uri.EscapeDataString(issuedPass)` mã hóa **giá trị** trước khi ghép vào `?pass=`. Ví dụ `+` thành `%2B`, `&` thành `%26`, `#` thành `%23`, `%` thành `%25`. Partner giải mã một lần và nhận đúng token ban đầu. Thay thế nội dung `starter/PartnerClient.cs` bằng file reference để tự kiểm chứng, hoặc tự triển khai cách tương đương.

## 5. Verification

Chạy `./verify.ps1` trên **starter đã sửa**, phải thấy `VERIFY_PASS: 6 of 6 contract cases`. Không chạy verify trên solution riêng. Chạy `./run.ps1` để xác nhận mọi case ACCEPTED.

## 6. Alternative fixes

- Dùng URI/query builder có API encode query-value rõ ràng; cần kiểm tra semantics thực tế của builder, tránh double-encoding.
- Thay token protocol thành Base64url nếu **cả issuer lẫn partner** được thay đổi và có versioned contract; không thể tự ý biến đổi token đối tác hiện tại.

## 7. Wrong / tempting fixes

- `Replace("+", "%20")`: đổi danh tính token và không xử lý `&`, `#`, `%`.
- `WebUtility.UrlEncode` trên **toàn bộ URI**: phá cấu trúc URL; encode value riêng mới là điều quan trọng. `UrlEncode` trên riêng value có thể hợp lệ nếu hai bên chấp nhận form-style encoding, nhưng phải test.
- Tăng retry hoặc cache kết quả: lặp lại request sai, không chữa contract.
- Chỉ test pass ASCII chữ/số: false confidence.

## 8. Production implications

Không log token thật ở dạng plaintext. Dùng correlation ID và digest an toàn cho so sánh tại ranh giới; tránh để query chứa bí mật trong access logs, telemetry, proxy history. Cân nhắc chuyển opaque secret sang header hoặc POST body nếu partner contract cho phép.

## 9. Trade-offs

Encoding query-value là sửa đổi nhỏ, tương thích ngược với partner parser. Đổi sang header/POST giảm rủi ro token xuất hiện trong URL nhưng đòi hỏi partner versioning, triển khai phối hợp và quan sát rollout.

## 10. Senior engineer should notice

Một lỗi xuất hiện theo tập con dữ liệu thường là dấu hiệu contract/encoding hơn là lỗi hạ tầng ngẫu nhiên. Cần chứng minh input–wire–decoded round-trip, bảo vệ trường hợp biên và kiểm tra rò rỉ token trong logs.

## References

- [Microsoft: Uri.EscapeDataString](https://learn.microsoft.com/dotnet/api/system.uri.escapedatastring)
- [Microsoft: WebUtility.UrlDecode](https://learn.microsoft.com/dotnet/api/system.net.webutility.urldecode)
- [RFC 3986 — URI Generic Syntax](https://www.rfc-editor.org/rfc/rfc3986)
