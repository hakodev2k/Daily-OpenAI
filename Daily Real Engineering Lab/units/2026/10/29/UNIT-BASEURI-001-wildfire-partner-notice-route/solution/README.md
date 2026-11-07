# Reference Solution — đọc sau khi tự reproduce và sửa

## 1. Symptoms
Hai endpoint versioned trả 404, root-only legacy trả 200; DNS/TLS bình thường.

## 2. Evidence
Adapter gửi https://dispatch.partner.invalid/notices?district=NW-4 thay vì https://dispatch.partner.invalid/v3/notices?district=NW-4. Backup cũng mất /integrations/v3/.

## 3. Root cause
HttpClient.BaseAddress kết hợp với request URI theo RFC 3986. Request target bắt đầu bằng / là absolute-path reference: thay toàn bộ path của BaseAddress. Do đó /notices làm mất versioned prefix.

## 4. Fix và vì sao hoạt động
Trong starter/Program.cs, PartnerRequests.BuildNoticeRequest phải tạo relative-path reference không có / ở đầu, để notices được resolve tương đối với BaseAddress kết thúc bằng /. Xem [reference source](Program.cs). Cấu hình BaseUrl cần trailing slash theo contract.

## 5. Verification
Chạy pwsh -NoProfile -File ./verify.ps1 trên starter đã sửa. Cả ba fixture phải trả HTTP 200. Fake handler so sánh method và absolute URI, bao gồm query. Reproduce chỉ chứng minh triệu chứng của starter gốc.

## 6. Alternative fixes
Có thể dùng URI builder với Origin và ApiPrefix tách riêng, hoặc build full URI được validate. Cả hai cần regression test về prefix và query. Leading slash vẫn đúng nếu resource thật sự nằm ở origin root.

## 7. Wrong / tempting fixes
- Retry 404 hoặc tăng timeout: không sửa request sai địa chỉ.
- Đổi BaseAddress thành host root để test xanh: bỏ mất API version và có thể gọi nhầm legacy endpoint.
- Nới fake handler để chấp nhận mọi path: chỉ che lỗi.
- Chỉnh query encoding: query đã hợp lệ, không giải thích được sự khác biệt theo BaseUrl.

## 8. Production implications
Thêm contract test cho prefix lồng nhau, legacy root và query. Log request URI đã redact secrets, status, environment, correlation ID; canary rollout, theo dõi 404 route-not-found, có rollback. Validate trailing slash cấu hình và chặn untrusted absolute URI.

## 9. Trade-offs
Relative request target đơn giản nhưng dựa vào invariant BaseUrl trailing slash. Explicit URI builder dài hơn và tăng cấu hình nhưng phù hợp API đa version.

## 10. Senior engineer nên nhận ra
HTTP 404 không đủ kết luận partner outage. Quan sát URI thực tế, phân tách authority/path/query, kiểm thử endpoint versioned và control để tránh fix tình cờ.

## References
- [RFC 3986 Section 5.2](https://www.rfc-editor.org/rfc/rfc3986#section-5.2)
- [Microsoft Learn HttpClient.BaseAddress](https://learn.microsoft.com/dotnet/api/system.net.http.httpclient.baseaddress)
