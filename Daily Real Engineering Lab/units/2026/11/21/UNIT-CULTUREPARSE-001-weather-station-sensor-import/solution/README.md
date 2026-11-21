# Reference solution — chỉ xem sau khi thử

## Root cause
`decimal.TryParse(string, out decimal)` dùng culture hiện hành. Dữ liệu wire định nghĩa dấu thập phân `.` nhưng worker chạy theo culture khác nhau; dấu chấm có thể bị từ chối hoặc hiểu như dấu nhóm. Hệ quả là dữ liệu hợp lệ không có cùng ngữ nghĩa giữa các region.

## Cách sửa
Giữ nguyên tách bản tin và validation, thay lời gọi parse trong `starter/Program.cs`:

```csharp
decimal.TryParse(fields[1], NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
    CultureInfo.InvariantCulture, out var temperature)
```

Không đổi `CurrentCulture` của toàn process để che lỗi. Không thay `.` thành `,` hoặc strip dấu phân cách. Chỉ cho phép grammar được mô tả trong protocol.

## Verification
Chạy `pwsh ./verify.ps1` sau khi sửa starter. Hai bản tin phải giữ đúng 12.75 và -3.50 dưới en-US, fr-FR và de-DE. Bổ sung regression cho malformed payload (ví dụ `12,75`, `1.2.3`, giá trị ngoài dải cảm biến) trước khi triển khai production.

## Wrong fixes
- `Replace(".", ",")`: thay đổi dữ liệu, không xử lý các culture khác và có thể nhận sai dấu nhóm.
- `CurrentCulture = en-US` toàn worker: ảnh hưởng logging, format UI và thư viện không liên quan.
- `decimal.Parse` không provider: giữ nguyên phụ thuộc môi trường, đồng thời ném exception cho dữ liệu xấu.

## Trade-offs
Hợp đồng wire bất biến giúp replay và audit đáng tin cậy. Nếu thiết bị legacy gửi nhiều grammar, cần version protocol rõ ràng, parser riêng theo version và telemetry lỗi; không đoán grammar theo locale triển khai.

## Tham khảo
- https://learn.microsoft.com/dotnet/api/system.decimal.tryparse
- https://learn.microsoft.com/dotnet/api/system.globalization.cultureinfo.invariantculture
- https://learn.microsoft.com/dotnet/api/system.globalization.numberstyles
