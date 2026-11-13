# Progressive hints

1. **Tín hiệu:** HTTP 429 và 503 có thể cung cấp thời điểm retry; kiểm tra cả status và header.
2. **Định dạng:** Đối chiếu cú pháp `Retry-After` trong RFC 9110 §10.2.3; không phải mọi giá trị đều là số nguyên.
3. **Thời gian:** Dùng `DateTimeOffset.TryParseExact` với định dạng `r`, `CultureInfo.InvariantCulture`, và tính chênh lệch từ tham số `now`; clamp [0,120] giây.
