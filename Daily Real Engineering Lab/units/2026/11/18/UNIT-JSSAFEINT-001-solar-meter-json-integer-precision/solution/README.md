# Reference Solution — chỉ đọc sau khi reproduce và thử sửa

## Symptoms
Hai mã `long` khác nhau bị client xem là một; UI chọn sai công tơ dù backend không trùng.

## Evidence
Wire chứa hai JSON number khác nhau; phía client chỉ còn một giá trị distinct. Số đo kWh vẫn chính xác trong ví dụ.

## Root cause
JavaScript `Number` dùng IEEE-754 binary64, chỉ biểu diễn chính xác mọi integer trong phạm vi ±(2^53−1). Giá trị 9007199254740993 không round-trip chính xác qua Number.

## Why the fix works
Đổi hợp đồng JSON: `Id` là opaque decimal string; `Kwh` vẫn là JSON number. Client so sánh identifier bằng string, không chuyển sang Number. Xem [reference code](Program.cs).

## Verify
Chép nội dung `solution/Program.cs` sang `starter/Program.cs` chỉ sau khi đã tự thử. Chạy `./verify.ps1`; cần DISTINCT_CLIENT_IDS=2, ID_ROUNDTRIP_OK=true, KWH_NUMERIC=true.

## Alternative fixes
Có thể dùng `BigInt` ở client nếu giao thức định nghĩa chuyển đổi string → BigInt và serialize lại rõ ràng. Dùng string thường đơn giản hơn cho opaque ID.

## Wrong / tempting fixes
- Dùng `decimal` chỉ ở backend: không giải quyết JavaScript Number.
- Làm tròn ID: tạo collision.
- Chuyển toàn bộ số đo thành string: tăng gánh nặng parsing và thay đổi contract không cần thiết.
- Dùng `BigInt` trực tiếp với JSON.stringify: không hỗ trợ mặc định.

## Production implications
Version schema, cập nhật OpenAPI/SDK, kiểm tra consumers cũ, dual-read trong giai đoạn migration và contract tests giữa backend/frontend.

## Trade-offs
String IDs tăng kích thước payload nhỏ và buộc validation dạng canonical decimal; đổi lại giữ nguyên identity và tương thích đa ngôn ngữ.

## Senior engineer should notice
Đây là lỗi ở boundary contract, không phải database uniqueness. Kiểm tra cả cache key, URL route, analytics export và browser UI. Tránh thay đổi loại trường measurement một cách tùy tiện.

## References
- MDN Number.MAX_SAFE_INTEGER: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number/MAX_SAFE_INTEGER
- RFC 8259, section 6: https://www.rfc-editor.org/rfc/rfc8259#section-6

