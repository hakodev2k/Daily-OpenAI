# Reference Solution — chỉ đọc sau khi tự verify

`Program.cs` trong thư mục này là phiên bản tham khảo. Chạy `dotnet run --project solution/BatchAudit.csproj` từ thư mục unit.

## Root cause
`await Task.WhenAll` throw khi ít nhất một task fault; các task khác vẫn có thể hoàn thành thành công. Catch ở cấp batch bỏ qua mọi kết quả riêng lẻ và gộp thành một rejected giả, khiến dashboard sai và retry trùng.

## Vì sao fix hoạt động
Materialize input IDs một lần, đợi toàn bộ tasks settle, phân loại từng task và giữ mapping ID. Một task fault không xoá kết quả thành công của task khác. Cần persist per-record outcomes trước khi retry các ID thất bại; dùng idempotency key phía partner để xử lý timeout không rõ kết quả.

## Wrong fixes
- Retry toàn batch: tạo duplicate đối với ID đã nhận.
- Bỏ catch và fail cả request: mất kết quả đối soát dù có task thành công.
- Dùng `Task.WhenAny` rồi chỉ đọc task đầu tiên: bỏ sót các kết quả còn lại.

## Trade-offs
Đối với hệ thống thật, lưu durable outcome, correlation ID, retry budget và trạng thái UNKNOWN cho timeout không chắc chắn. Demo chỉ mô phỏng success/fault; không mô phỏng ambiguous network acknowledgments.
