# Reference Solution — SPOILER

**Chỉ đọc sau khi đã reproduce, ghi hypothesis và tự thử fix.**

## 1. Symptoms
Dashboard báo hai lần gửi thành công, nhưng hệ thống đối tác không lưu receipt nào.

## 2. Evidence
Trace mô phỏng: `POST /legacy/evidence → 302`, tiếp theo `GET /api/evidence → 200 text/html`. Lần gửi vào decoy cũng kết thúc `200 text/html`. `partner.ReceivedIds` rỗng. HTTP 2xx không chứng minh đã thực hiện nghiệp vụ.

## 3. Root cause
`HttpClientHandler.AllowAutoRedirect` mặc định bật. Với `302 Found`, client có thể chuyển `POST` thành `GET` khi theo redirect. `IsSuccessStatusCode` chỉ kiểm tra HTTP status cuối, trong khi starter không xác minh receipt. Đây là kết hợp giữa method downgrade và success oracle quá yếu.

## 4. Why fix works
[PartnerGateway.cs](PartnerGateway.cs) tắt auto-redirect, kiểm tra `Location` bằng parsed URI (scheme, host, port, path, userinfo, query, fragment), chỉ cho phép một đích ingest được cấu hình, chủ động gửi lại POST và kiểm tra receipt ID khớp evidence ID. Không coi trang HTML 200 là biên nhận.

## 5. Verify
Chỉnh `starter/PartnerGateway.cs`, chạy `./scripts/verify.ps1`. Exit 0 khi một receipt thật `RCPT-EV-217` được lưu, decoy bị từ chối, không có receipt cho EV-218. Script chạy starter, **không** chạy reference. Để tự đối chiếu, sau khi đã thử fix có thể copy reference file vào starter rồi chạy verify.

## 6. Alternative fixes
Nếu hợp đồng API thuộc quyền kiểm soát, partner nên dùng `307/308` khi cần giữ nguyên method và body. Một giải pháp khác là bỏ endpoint legacy, gọi thẳng canonical ingest URL được cấu hình và vẫn kiểm tra typed receipt. Với hệ thống bên thứ ba, fail-closed và cập nhật contract có thể an toàn hơn tự động replay body.

## 7. Wrong / tempting fixes
- Tăng timeout hoặc retry không làm POST đến đúng endpoint; còn có nguy cơ nhân đôi side effect nếu đối tác đã nhận.
- Chỉ kiểm tra `200 OK` hoặc `IsSuccessStatusCode` vẫn có thể chấp nhận HTML landing page.
- Tự động POST lại tới **mọi** Location có thể gửi dữ liệu nhạy cảm sang host không tin cậy.
- Đổi mọi redirect thành `307` từ phía client mà không có contract với server không xử lý trust boundary.

## 8. Production implications
Gắn idempotency key theo evidence ID khi replay POST, xác thực TLS/canonical origin, giới hạn redirect hops, log method/status/content-type mà không log ảnh hay token. Đo metric `partner_receipt_confirmed` riêng với `http_success`.

## 9. Trade-offs
Manual redirect logic thêm mã và phải duy trì allowlist; gọi canonical endpoint trực tiếp đơn giản hơn nếu hợp đồng ổn định. Nếu partner không cung cấp receipt đáng tin cậy, không được tuyên bố exactly-once hoặc confirmed delivery.

## 10. Senior engineering insight
Một request HTTP thành công chỉ là bằng chứng tầng giao thức. Trạng thái nghiệp vụ phải dựa trên acknowledgement có định danh và chính sách retry/idempotency được thiết kế theo side effects. Luôn kiểm tra effective method sau redirect, không chỉ final status.

## Tài liệu tham khảo
- [HttpClientHandler.AllowAutoRedirect](https://learn.microsoft.com/dotnet/api/system.net.http.httpclienthandler.allowautoredirect)
- [RFC 9110 — 302 Found](https://www.rfc-editor.org/rfc/rfc9110.html#section-15.4.3)
- [RFC 9110 — 307 Temporary Redirect](https://www.rfc-editor.org/rfc/rfc9110.html#section-15.4.8)
