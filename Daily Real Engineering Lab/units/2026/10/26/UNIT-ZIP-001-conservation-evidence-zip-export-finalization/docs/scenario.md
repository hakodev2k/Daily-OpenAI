# Scenario và evidence cần thu thập

**Incident ticket:** CONS-EXP-241. Bộ phận lưu trữ nhận file export ZIP nhưng ứng dụng desktop báo “Cannot open archive”. Bản ghi export trong backend không có exception; số byte trả về khác nhau theo số tài liệu.

**Hợp đồng nghiệp vụ:** gói có đúng hai entry `manifest.json`, `reports/site-inspection.txt` với nội dung giữ nguyên; gói rỗng vẫn là một ZIP hợp lệ có 0 entry. Dữ liệu chạy local, không chứa PII.

**Điều tra:** Chạy `scripts/run.ps1`, lưu lại status và số byte. Chạy `scripts/reproduce.ps1` để kiểm chứng lỗi; sau đó quan sát thứ tự các thao tác trên luồng tạo dữ liệu và kiểm tra xem consumer báo lỗi cấu trúc hay sai nội dung.

**Nhiễu có chủ ý:** tên file có thư mục con, encoding UTF-8, compression level và thứ tự entry đều có thể đáng nghi nhưng không đủ bằng chứng để kết luận.

**Success criteria:** ZIP đọc được bằng `ZipArchiveMode.Read`, đúng tên và nội dung entry, gói rỗng hợp lệ, không cần dịch vụ bên ngoài.
