# Reference Solution — Spoiler
**Chỉ xem sau khi đã reproduce và thử sửa.**

## Symptoms
Phiếu XML hợp lệ được import, nhưng một phiếu có phần khai báo bất thường khiến ghi chú chứa chuỗi canary vốn chỉ nằm trong file local giả lập.

## Evidence
`Program.cs` tạo file temp với canary và xây dựng XML tham chiếu đến URI của file. `ReceiptXmlParser` sử dụng một reader cho phép tài liệu XML mở rộng dữ liệu ngoài tài liệu gốc.

## Root cause
`DtdProcessing.Parse` cùng `XmlUrlResolver` cho phép external entity trong DTD được resolve qua `file:` URI. Đối tác có thể kiểm soát XML; dữ liệu local không nằm trong trust boundary của payload. Giới hạn `MaxCharactersFromEntities=1024` không phải chính sách cấm truy cập tài nguyên ngoài.

## Why the fix works
Cấu hình `DtdProcessing.Prohibit`, `XmlResolver=null` và không cho phép entity expansion. Khi XML chứa `DOCTYPE`, `XmlReader` ném `XmlException`; XML thông thường vẫn parse được.

## How to verify
1. Copy nội dung `solution/ReceiptXmlParser.cs` vào `starter/ReceiptXmlParser.cs` **chỉ để đối chiếu sau khi tự làm**.
2. Chạy `./verify.ps1`: `VALID_RECEIPT=PASS`, `EXTERNAL_ENTITY=REJECTED`, exit code 0.
3. Không dùng `reproduce.ps1` để chứng minh fix: script đó chứng minh trạng thái lỗi ban đầu.

## Alternative fixes
- `DtdProcessing.Ignore` và `XmlResolver=null` có thể phù hợp nếu hợp đồng yêu cầu bỏ qua DTD, nhưng cần xác minh các trường dùng entity nội bộ không bị biến dạng.
- Nếu thật sự cần DTD từ một đối tác tin cậy, phải thiết kế resolver allowlist offline, giới hạn tài nguyên, kiểm thử kỹ và ghi nhận ngoại lệ chính sách. Không bật `XmlUrlResolver` rộng rãi cho XML không tin cậy.

## Wrong / tempting fixes
- Chỉ xóa canary khỏi log: che dấu symptom, dữ liệu đã đi qua parser.
- Chỉ đặt `MaxCharactersFromEntities` thấp: hạn chế độ lớn, không chặn đọc file nhỏ.
- Chỉ escape nội dung `Note` sau khi parse: external access đã xảy ra trước đó.
- Tắt mọi XML import: an toàn tức thời nhưng không đáp ứng business contract; có thể dùng tạm khi incident chưa được khoanh vùng.

## Production implications
Rà soát toàn bộ điểm vào XML, quyền đọc file của service account, lịch sử dữ liệu đã import và chính sách đối tác. Không ghi secret hoặc payload nguyên bản vào telemetry. Dùng audit log redacted và test regression ở CI.

## Trade-offs
Chặn DTD có thể làm hỏng đối tác dùng internal entities hợp lệ. Cần hợp đồng schema, migration plan, khả năng rollback có kiểm soát và giám sát số lượng payload bị từ chối.

## Senior engineer should notice
Security boundary nằm ở parser configuration, không nằm ở business validation của `Note`. Kiểm thử phải xác nhận cả input hợp lệ và input tấn công, đồng thời phân biệt `reproduce` với `verify`.

**Validation:** chỉ static-review trong lần xuất bản này; chưa thực thi .NET/PowerShell.
