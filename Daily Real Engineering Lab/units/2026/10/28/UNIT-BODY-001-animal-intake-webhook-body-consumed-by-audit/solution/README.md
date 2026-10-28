# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
Hai POST JSON hợp lệ có audit payload đầy đủ, nhưng endpoint thấy chuỗi rỗng và trả 422.

## Evidence
`reproduce.ps1` build thành công, in `auditBytes > 0`, `endpointBytes = 0`, `status = 422`, rồi `REPRO_FAIL`. Fixture dùng stream **không seek** giống đặc tính phổ biến của request body thực tế.

## Root cause
Middleware đọc `Request.Body` đến cuối stream. Endpoint tiếp tục đọc **cùng** stream tại vị trí hiện tại. `leaveOpen: true` chỉ giữ stream mở, không tự rewind. Trên non-seekable stream, đặt `Position = 0` trực tiếp cũng không hợp lệ.

## Fix và lý do hoạt động
Thay `starter/RequestAuditMiddleware.cs` bằng `solution/RequestAuditMiddleware.cs`. `EnableBuffering` phải được gọi trước lần đọc đầu tiên, cho phép đọc lại nội dung đã buffer; middleware đưa `Position` về 0 trước khi chuyển quyền cho endpoint. `leaveOpen: true` tránh đóng stream thuộc sở hữu request pipeline. Demo giới hạn 64 KiB để tránh buffer vô hạn.

## Verify
Chạy `./verify.ps1` sau khi sửa starter. Hai request phải có `status=200`, `auditBytes=endpointBytes=expectedBytes`, và `VERIFY_PASS`. Script chỉ kiểm tra starter đã chỉnh sửa, không tự thay bằng reference.

## Alternative fixes
- Audit ở tầng endpoint sau khi deserialize, ghi metadata hoặc một bản snapshot đã được kiểm soát; ít đọc lặp hơn nhưng đổi kiến trúc.
- Chuyển sang streaming audit có giới hạn và tee stream, phức tạp hơn và cần xử lý partial reads.
- Nếu audit chỉ cần metadata, tránh đọc body hoàn toàn.

## Wrong / tempting fixes
- Chỉ dùng `leaveOpen:true`: không phục hồi cursor.
- Chỉ gọi `Position=0` trên stream gốc: ném `NotSupportedException` nếu non-seekable.
- Xóa audit: che triệu chứng, vi phạm yêu cầu.
- Buffer toàn bộ body không giới hạn: có nguy cơ memory/disk exhaustion.

## Production implications / trade-offs
`EnableBuffering` có chi phí bộ nhớ và có thể spill xuống disk; đặt body-size limit và xử lý `IOException` khi vượt `bufferLimit`. Không log payload chứa PII, secret hoặc dữ liệu y tế. Kiểm tra charset, body nén, request cancellation và hành vi khi endpoint stream dữ liệu lớn. Không khuyến nghị buffer mọi request nếu chỉ một nhóm webhook cần audit.

## Senior engineering observations
Phân biệt ownership, stream position và lifecycle; kiểm thử với non-seekable stream thay vì MemoryStream thông thường; thiết kế audit policy có giới hạn tài nguyên và redaction; xác định hợp đồng lỗi (400/413/422) rõ ràng. `EnableBuffering` không tự rewind sau khi middleware đọc.
