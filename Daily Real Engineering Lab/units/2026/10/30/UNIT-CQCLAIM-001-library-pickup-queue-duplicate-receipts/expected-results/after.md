# After — tiêu chí nghiệm thu
Cả hai fixture hai yêu cầu trả về chính xác hai ID khác nhau, mỗi ID một lần, bất kể thứ tự. Fixture một yêu cầu trả đúng một ID; hàng đợi rỗng không tạo phiếu. `verify.ps1` exit 0 trên code trong `starter/`; `reproduce.ps1` exit 1 vì triệu chứng gốc không còn.

Bảo đảm này chỉ áp dụng cho một hàng đợi trong một process. Khi dùng queue phân tán, cần thêm durable claim/ack, retry, idempotency, crash recovery và bảo vệ tác dụng phụ ở hệ thống nhận.
