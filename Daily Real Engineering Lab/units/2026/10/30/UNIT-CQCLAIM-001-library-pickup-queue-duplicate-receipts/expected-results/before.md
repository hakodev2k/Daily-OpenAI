# Before — triệu chứng mong đợi
Với mỗi fixture hai yêu cầu, starter trả về hai phiếu nhưng cả hai mang cùng ID của yêu cầu đầu tiên. ID thứ hai không có phiếu. Không cần exception, CPU spike hay network để tái hiện. Fixture một yêu cầu và rỗng vẫn đúng. `reproduce.ps1` phải exit 0; `verify.ps1` phải exit 1.

Nếu không reproduce: kiểm tra đang chạy từ đúng unit root, .NET SDK 8, không sửa harness và không vô tình dùng file solution thay cho starter. Kết quả không phụ thuộc thời gian máy vì harness chủ động đồng bộ hai worker.
