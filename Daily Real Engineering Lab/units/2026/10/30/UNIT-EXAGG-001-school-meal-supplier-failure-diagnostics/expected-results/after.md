# After — hợp đồng cần đạt
- Bình thường: publish true, tổng 7, không có diagnostic.
- Hai nguồn faulted: publish false, tổng 0, đúng hai chẩn đoán `orchard:HTTP_503` và `dairy:INVALID_SCHEMA` (không phụ thuộc thứ tự).
- Chỉ có cancellation: `OperationCanceledException` tiếp tục được truyền lên caller.
- `verify.ps1` exit 0; `reproduce.ps1` không còn tìm thấy triệu chứng nên exit 1.
- Không cần gọi network hay chạy nhiều luồng thật; lỗi được kích hoạt bằng tác vụ hoàn tất có kiểm soát.
