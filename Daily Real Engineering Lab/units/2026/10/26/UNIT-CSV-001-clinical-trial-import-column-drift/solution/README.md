# Reference Solution — chỉ xem sau khi tự reproduce và thử sửa

## 1. Symptoms
Bản ghi TR-101 có nhiều hơn 4 cột, TR-103 bị chia thành nhiều record, TR-102 không bảo toàn dấu nháy kép. Import có thể làm sai lệch thông tin thử nghiệm, gây rủi ro audit.

## 2. Evidence
`reproduce.ps1` cho thấy sai cả **record count** lẫn **field values**. Fixture cố định, không có network/database, nên nguyên nhân không phải retry hoặc transaction.

## 3. Root cause
Starter tách file bằng `Split('\\n')` rồi tách field bằng `Split(',')`. CSV cho phép dấu phẩy, CRLF và escaped quotes **bên trong quoted fields**. Ký tự chỉ có ý nghĩa phân cách theo trạng thái cú pháp, không phải ở mọi vị trí.

## 4. Why the fix works
[CsvParser.cs](CsvParser.cs) quét từng ký tự với trạng thái `insideQuotes`, `closedQuote`; dấu phân cách và newline chỉ tạo boundary khi ở ngoài quoted field. `""` được chuyển thành `"` trong nội dung; quoted field chưa đóng hoặc ký tự bất hợp lệ sau closing quote bị từ chối.

## 5. How to verify
Chép nội dung reference `CsvParser.cs` sang `starter/TrialImport/CsvParser.cs` **chỉ sau khi đã tự làm**, rồi chạy `./verify.ps1`. Lệnh kiểm tra code starter, không tự chạy reference. Sau khi sửa, `reproduce.ps1` phải không còn tái hiện triệu chứng.

## 6. Alternative fixes
Dùng thư viện CSV chuyên dụng có phiên bản pin, contract tests và giới hạn kích thước input là lựa chọn production thường tốt hơn tự duy trì parser. Lab dùng BCL thuần để học rõ boundary. Một parser streaming trên `TextReader` phù hợp file lớn hơn so với đọc toàn bộ string.

## 7. Wrong / tempting fixes
- `Replace(",", ";")`: phá dữ liệu hợp lệ.
- Bỏ record không đúng 4 cột: mất hồ sơ và che lỗi dữ liệu.
- `Trim()` mọi field: thay đổi nội dung nghiệp vụ.
- `Split('\\n')` rồi thêm logic đặc biệt cho một trường: không xử lý quoted newline tổng quát.
- Chỉ tăng số cột cho phép: làm sai schema downstream.

## 8. Production implications
Giới hạn kích thước file, số field, chiều dài field và số record; log `case_id` hoặc hash định danh có kiểm soát, không log ghi chú nhạy cảm. Quarantine file lỗi; đo tỷ lệ reject; ghi version hợp đồng dữ liệu và bảo toàn bytes đầu vào cho audit theo chính sách retention.

## 9. Trade-offs
Parser thủ công không có dependency nhưng tăng maintenance/security surface. Thư viện chuẩn đã kiểm chứng giảm rủi ro edge cases nhưng cần pin version, đánh giá behavior về BOM, encoding, dialect và malformed input.

## 10. Senior engineer should notice
Sai một ký tự parsing có thể biến thành lỗi **data integrity**, không chỉ lỗi giao diện. Phải kiểm chứng chính xác dữ liệu, fail-closed cho input sai và có regression tests từ sample thực tế đã ẩn thông tin cá nhân.

Tham khảo: RFC 4180 (Common Format and MIME Type for CSV Files); .NET `StringBuilder` và `FormatException` documentation.
