# Spoiler

Root cause: duplicate JSON property được deserialize thành một giá trị cuối; không có lỗi mặc định. Reference kiểm tra trùng property name trước khi mapping và reject. Production cần kiểm tra nested object, giới hạn kích thước/độ sâu và policy case sensitivity.

## Wrong fixes
Try/catch Deserialize không đủ vì deserialize thành công. Chọn first/last tùy ý tạo bất đồng giữa hệ thống. Chỉ log warning không ngăn dữ liệu sai.

## Trade-offs
Reject strict cần quarantine, correlation ID và hợp đồng với upstream; không tự sửa giá trị tài chính.
