# Reference solution — spoiler

## Root cause
`Task.WhenAny` chỉ xác định task nào hoàn thành trước; trả về `TIMEOUT` không hủy operation thua cuộc. Khi ACK cũ đến sau lệnh mới, tác động phụ cũ vẫn ghi đè trạng thái.

## Phương án
Mỗi yêu cầu lấy một generation tăng đơn điệu. Chỉ generation hiện hành được ghi trạng thái. Code tham khảo áp dụng fencing ở điểm ghi. Trong production, phải thực thi fencing ở thiết bị/server sở hữu side effect, không chỉ trong memory của một process; truyền command ID, kiểm tra thứ tự và persist trạng thái. Cancellation giúp giảm công việc thừa nhưng không thay thế fencing (thiết bị có thể đã nhận lệnh).

## Trade-offs
Generation in-memory chỉ bảo vệ một instance; restart hoặc nhiều replicas cần sequence lưu bền và kiểm tra atomic. ACK đến trễ cần log command ID và correlation ID. `Task.WhenAny` cũng cần quan sát lỗi của task thua cuộc; demo cố ý tối giản vì không phát sinh exception.

## Verify
`./verify.ps1` chạy reference và kiểm tra `mode after old ack: ACTIVE`. Sau đó sửa starter và chạy lại reproduce; script verify chỉ là baseline tham khảo, không chứng minh thay đổi trong starter.
