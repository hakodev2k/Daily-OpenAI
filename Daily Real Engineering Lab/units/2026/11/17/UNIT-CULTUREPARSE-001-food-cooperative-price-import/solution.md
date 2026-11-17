# Reference Solution — chỉ xem sau khi reproduce và tự sửa

## 1. Symptoms
Cùng chuỗi giá `1,234.50` cho kết quả khác nhau hoặc `FormatException` tùy culture của worker.

## 2. Evidence
Chạy `pwsh ./reproduce.ps1`; lưu raw payload, `CurrentCulture.Name`, kết quả parse/exception. `en-US` chấp nhận chuỗi, `de-DE` và `fr-FR` không diễn giải theo cùng wire contract.

## 3. Root cause
`decimal.Parse(raw)` sử dụng `CultureInfo.CurrentCulture` khi không truyền provider. Locale máy chủ không phải đặc tả format của nhà cung cấp.

## 4. Reference fix
Thay đúng phương thức `ParsePartnerPrice` trong `starter/Program.cs` bằng:

```csharp
private static decimal ParsePartnerPrice(string raw)
{
    // Partner contract: ASCII digits, optional comma groups, dot decimal.
    if (string.IsNullOrWhiteSpace(raw) ||
        !System.Text.RegularExpressions.Regex.IsMatch(
            raw, @"^\d{1,3}(,\d{3})*(\.\d{1,2})?$|^\d+(\.\d{1,2})?$",
            System.Text.RegularExpressions.RegexOptions.CultureInvariant))
        throw new FormatException("Invalid partner price format");

    return decimal.Parse(
        raw,
        NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint,
        CultureInfo.InvariantCulture);
}
```

Trong hệ thống thực tế cần định nghĩa thêm chính sách dấu âm, số chữ số thập phân, giới hạn giá trị, Unicode digits và xử lý overflow theo hợp đồng dữ liệu. Regex ở đây giới hạn ASCII digits và tối đa 2 chữ số thập phân; nên giữ test fixtures cho mọi edge case.

## 5. Verification
Chạy `pwsh ./verify.ps1` sau khi sửa starter. Test kiểm tra 4 locale, tổng 11110.75 và từ chối 3 input sai. Nếu thất bại, xác nhận sửa đúng file và SDK 8 có sẵn.

## 6. Alternative fixes
Sử dụng `decimal.TryParse` với explicit provider và strict format validator để trả lỗi theo từng record thay vì throw; thích hợp batch ingestion có quarantine.

## 7. Wrong / tempting fixes
- Đổi locale toàn bộ server sang en-US: che giấu dependency và ảnh hưởng các phần mềm khác.
- `Replace(",", "")` cho mọi input: vô tình chấp nhận format đối tác không hợp lệ, làm mất khả năng phát hiện dữ liệu lỗi.
- `double.Parse` thay decimal: tăng rủi ro sai số tiền tệ.
- Bắt exception rồi trả 0: làm sai đối soát và che giấu dữ liệu hỏng.

## 8. Production implications
Gắn source format version vào ingestion, log lỗi có mã record (không log PII), quarantine malformed rows, đo tỉ lệ reject và thêm contract tests chạy với nhiều culture.

## 9. Trade-offs
Strict validation tăng độ tin cậy nhưng có thể từ chối payload lịch sử không chuẩn. Nếu đối tác đổi format, cần versioned contract và migration rõ ràng.

## 10. Senior engineering judgment
Không phụ thuộc default runtime locale tại system boundary. Phân biệt **parsing**, **validation** và **display formatting**; giữ dữ liệu số dưới dạng decimal xuyên suốt domain, chỉ format khi xuất ra UI.
