# Reference Solution — spoiler

**Chỉ đọc sau khi đã reproduce, tự sửa và verify.**

## 1. Symptoms
Caption API chọn sai locale ở những request chứa nhiều language-range, mức ưu tiên khác nhau hoặc subtag vùng. API trả 200 nhưng nội dung không đúng mong đợi.

## 2. Evidence
R-201/R-202 cho kết quả trái với mức chấp nhận; R-203/R-204 cho thấy lựa chọn fallback không theo danh sách hỗ trợ. Request không có header vẫn đúng, CPU và 5xx không gợi ý sự cố hạ tầng.

## 3. Root cause
Starter chọn token đầu tiên sau khi tách dấu phẩy, không giải thích `q` hay tập lựa chọn có thể phục vụ. Một số phép kiểm tra tiền tố cũng không phù hợp với ngôn ngữ có subtag vùng.

## 4. Why the fix works
[CaptionLocale.cs](CaptionLocale.cs) phân tích từng range thành `language + q + position`, ánh xạ ngôn ngữ vùng về base language được hỗ trợ (`fr-CA` → `fr`), ưu tiên explicit range so với wildcard, loại bỏ `q=0`, chọn theo trọng số giảm dần và thứ tự client công bố. Tập locale `en/vi/fr` là allowlist; không có lựa chọn hợp lệ thì trả `null`.

Đây là **hợp đồng thương lượng rút gọn** của hệ thống caption, không phải một implementation BCP 47/RFC 4647 đầy đủ: chỉ hỗ trợ primary-language fallback, chưa xử lý lookup phức tạp hoặc language-range dài hơn. Duplicate base-language ranges được chọn theo `q` cao nhất; cần xác định chính sách riêng nếu product cho phép header mâu thuẫn.

## 5. How to verify
Giữ nguyên `harness/Program.cs`. Chép nội dung reference vào `starter/CaptionLocale.cs` **chỉ sau khi tự thử**, chạy `./scripts/verify.ps1`. Kết quả 16 PASS, exit 0. `./scripts/reproduce.ps1` vẫn phải báo REPRODUCED vì baseline tách riêng.

## 6. Alternative fixes
- Dùng thư viện content negotiation có kiểm chứng, kèm adapter về tập locale `en/vi/fr`.
- Trả default `en` khi không match là chính sách sản phẩm có thể chọn, **nhưng** khác hợp đồng lab (trả `null` → HTTP 406); cần thương lượng với client.
- Lưu cấu hình locale người dùng trên server chỉ hợp lý nếu định nghĩa precedence rõ giữa profile và header.

## 7. Wrong / tempting fixes
- Đổi `StartsWith` thành `Contains`: có thể vô tình match token không được chấp nhận, vẫn bỏ qua q.
- Chỉ đảo danh sách `vi,en,fr`: làm đúng vài request nhưng phá request khác.
- Thêm Redis hoặc tăng instance: không thay đổi quy tắc chọn locale.
- Chỉ thêm case `fr-CA`: vá một triệu chứng mà bỏ qua `q=0` và wildcard.

## 8. Production implications
Tách selection khỏi việc tải caption; trả `Content-Language` theo locale thực tế, quan sát tỷ lệ 406 và các ngôn ngữ được chọn, giới hạn độ dài header, không log dữ liệu nhạy cảm. Nếu response được cache, phải bảo đảm cache key hoặc `Vary: Accept-Language` phản ánh chính sách lựa chọn.

## 9. Trade-offs
Parser nhỏ giúp kiểm soát hợp đồng nhưng tăng chi phí bảo trì và độ phủ edge case; thư viện chuẩn có thể xử lý nhiều trường hợp hơn nhưng phải kiểm tra tương thích. Hỗ trợ wildcard và regional fallback giúp UX tốt hơn nhưng cần kiểm soát yêu cầu compliance về ngôn ngữ.

## 10. What a Senior engineer should notice
Một header hợp lệ không đảm bảo có representation phù hợp. Hợp đồng cần phân biệt không có header, lựa chọn không hỗ trợ, lựa chọn bị loại trừ, malformed input và cache variation. Chứng minh tính đúng bằng behavior contract thay vì chỉ nhìn response 200.

## References
- [RFC 9110, Accept-Language](https://www.rfc-editor.org/rfc/rfc9110.html#section-12.5.4)
- [RFC 4647, Matching of Language Tags](https://www.rfc-editor.org/rfc/rfc4647.html)
- [Microsoft Learn — StringWithQualityHeaderValue](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.headers.stringwithqualityheadervalue)
