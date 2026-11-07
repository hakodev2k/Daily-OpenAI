# Reference Solution

> Chỉ xem sau khi đã reproduce và thử sửa.

## 1. Symptoms
Cùng chuỗi giá từ partner cho kết quả khác nhau khi process chạy dưới culture khác.

## 2. Evidence
Input không đổi; chỉ `CurrentCulture` đổi. Parser đang dùng overload phụ thuộc ambient culture.

## 3. Root cause
Machine-formatted decimal data có contract dùng dấu chấm, nhưng code gọi `decimal.TryParse(input, out value)`, khiến interpretation phụ thuộc culture của process.

## 4. Why the fix works
Đưa format contract vào parsing boundary, ví dụ dùng `CultureInfo.InvariantCulture` với `NumberStyles.Number`, làm kết quả độc lập với regional settings.

Ví dụ:
```csharp
var ok = decimal.TryParse(
    input,
    NumberStyles.Number,
    CultureInfo.InvariantCulture,
    out var value);
```

## 5. How to verify
Chạy `./verify.ps1`; cả `en-US` và `de-DE` phải tạo `19.95` và output cuối là `VERIFY_PASS`.

## 6. Alternative fixes
Nếu partner quy định culture cụ thể, dùng culture đó thay vì `InvariantCulture`. Với JSON chuẩn, ưu tiên serializer/parser theo JSON number grammar thay vì tự parse chuỗi.

## 7. Wrong or misleading fixes
Ép toàn process sang một culture có thể ảnh hưởng formatting/UI và các integration khác. Thay dấu phẩy/dấu chấm bằng string replacement không mô hình hóa đầy đủ number format và có thể phá thousands separator.

## 8. Production implications
Lỗi globalization thường chỉ xuất hiện trên build agent, container hoặc server ở region khác, gây import sai dữ liệu dù test local pass.

## 9. Trade-offs
Explicit culture làm contract rõ ràng nhưng yêu cầu biết chính xác format upstream. Nếu upstream gửi localized text, contract phải bao gồm locale thay vì giả định invariant.

## 10. What a Senior engineer should notice
Culture là dependency ẩn. Boundary đọc machine data nên khai báo encoding, timezone, culture và numeric/date format rõ ràng, đồng thời test dưới nhiều environment settings.