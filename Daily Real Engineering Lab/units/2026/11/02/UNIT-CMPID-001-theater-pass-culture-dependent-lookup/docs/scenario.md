# Scenario — Vé nhà hát trên kiosk đa vùng

Nhà hát có hai cửa quét cùng build ứng dụng. Mã in trên vé là `INV-42`; nhân viên có thể nhập `inv-42` khi máy quét hỏng. Một kiosk nhận vé, kiosk khác báo không tìm thấy. Báo cáo nhập vé cũng ghi nhận có thể thêm hai mã mà vận hành xem là trùng.

**Hợp đồng nghiệp vụ:** mã vé gồm ASCII `A-Z`, chữ số và dấu gạch ngang; người dùng được phép nhập biến thể ASCII hoa/thường. Ký tự ngoài ASCII không được xem là cùng mã, kể cả hình dạng gần giống. Trong production nên validate định dạng ở biên API; lab tập trung vào equality và dictionary key.

**Giới hạn mô phỏng:** không có QR scanner, database, thanh toán, dữ liệu khách hàng hay endpoint thật. Harness tạo registry mới trong mỗi locale để không trộn state và kiểm tra hành vi một cách quyết định.

**Nhiệm vụ điều tra:** phân biệt lỗi nhập dữ liệu, lỗi seed, và lỗi định nghĩa identity. Không thay đổi ngữ nghĩa nghiệp vụ chỉ để pass một locale.
