# Reference Solution — SPOILER

Chỉ đọc sau khi reproduce, ghi hypothesis và tự sửa `starter/`.

## Symptoms
CSV đúng số dòng, cột và Amount nhưng 6 DisplayName có nguy cơ được spreadsheet diễn giải như biểu thức.

## Evidence
`reproduce.ps1` kiểm tra **parsed CSV field values** và báo 6 formula-like text cells; normal names, comma, quote và newline round-trip đúng. Không có spreadsheet formula nào được thực thi.

## Root cause
CSV quoting/escaping chỉ bảo vệ **cú pháp field**, không quy định **spreadsheet cell type**. Dữ liệu DisplayName do đối tác nhập đi thẳng vào CSV; ký tự mở đầu mang ý nghĩa công thức, kể cả sau leading whitespace, có thể bị client diễn giải khác text thuần. Đây là CSV/spreadsheet formula injection.

## Why the fix works
Áp dụng text-cell policy cho untrusted DisplayName **trước** CSV escaping: nếu ký tự đầu có ý nghĩa sau space/tab/CR/LF là `= + - @`, thêm text prefix vào output. Sau đó serialize CSV như bình thường. Numeric Amount vẫn là số; BeneficiaryId được cấp bởi hệ thống; database không đổi. Xem [reference code](GrantCsvExporter.cs), có thể đối chiếu với starter sau khi tự sửa.

## How to verify
Chạy `./verify.ps1` trên **starter đã chỉnh sửa**: 9 records, 3 cột, Amount invariant, normal text nguyên vẹn, 6 risky fields không còn formula-like sau CSV parse. Đây là deterministic contract test, **không chứng minh** mọi spreadsheet client xử lý prefix giống nhau.

## Alternative fixes
- **XLSX explicit string cells**: tốt hơn khi cần typed cells; thêm dependency, format compatibility và test matrix.
- **Controlled CSV import với text-column mapping**: khả thi nếu kiểm soát được quy trình import.
- **Validate/reject input**: chỉ đúng nếu business thật sự cấm những tên đó, không nên sửa stored data chỉ để chữa lỗi export.

## Wrong / tempting fixes
- Chỉ bao field bằng double quotes: đúng CSV syntax nhưng không ép spreadsheet xem là text.
- Xóa `= + - @` khỏi database: phá dữ liệu gốc và audit.
- Chỉ lọc dấu `=` ở vị trí đầu: bỏ sót prefix khác và whitespace.
- Chuyển Amount thành text: phá numeric semantics.
- Coi parser CSV pass là bằng chứng an toàn tuyệt đối: parser và spreadsheet có semantics khác nhau.

## Production implications
Threat model phải xét nguồn untrusted, client import, re-save và machine consumers. Test client thật trong môi trường cô lập, không dùng payload truy cập mạng. Version hóa export contract, tránh log PII, giữ nguyên raw input.

## Trade-offs
Apostrophe prefix có thể hiện như ký tự thật, bị thay đổi sau round-trip hoặc xử lý khác nhau tùy client. CSV không thể áp đặt cell type chuẩn hóa; nếu cần bảo đảm, cân nhắc XLSX typed cells hoặc controlled import.

## Senior engineer should notice
Phân biệt syntactic correctness, semantic interpretation, trust boundary và compatibility; test cả whitespace, comma, quote, newline, numeric precision, client behavior và downstream consumers.

## References
- [OWASP CSV Injection](https://owasp.org/www-community/attacks/CSV_Injection)
- [RFC 4180](https://www.rfc-editor.org/rfc/rfc4180)
- [Microsoft String.TrimStart](https://learn.microsoft.com/en-us/dotnet/api/system.string.trimstart)
