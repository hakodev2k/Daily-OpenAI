# Investigation Guide

1. Chạy `./reproduce.ps1` và ghi lại input/output từng ca.
2. Phân loại triệu chứng: dòng thiếu, dòng dư hay sai thứ tự?
3. Theo dấu đăng ký dịch vụ, resolve worker, đường đi `Export` và trạng thái object.
4. Xác định object nào được dùng lại giữa hai lượt xử lý.
5. Dự đoán kết quả của thay đổi đề xuất, sau đó chạy `verify.ps1`.
6. Nếu output đúng nhưng strict DI validation thất bại, kiểm tra ownership của dependency.
7. Đánh giá liệu cách sửa có an toàn khi hai job chạy đồng thời không.
