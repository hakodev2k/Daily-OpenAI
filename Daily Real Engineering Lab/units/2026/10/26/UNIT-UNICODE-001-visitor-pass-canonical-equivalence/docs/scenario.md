# Tình huống và tiêu chí nghiệp vụ
Bảo tàng nhận log quét vé từ hai nền tảng nhập liệu độc lập. Dashboard phải báo cáo số **định danh vé duy nhất**, không phải số sự kiện nhận được. Mã vé phân biệt chữ hoa/chữ thường; lần quét đầu tiên phải thắng; không được sửa payload gốc khi loại trùng. Không có kết nối bên ngoài.

**Dữ liệu:** bốn sự kiện cố định, gồm hai sự kiện trông cùng mã nhưng do thiết bị khác nhau gửi, một mã khác case và một lần quét lặp y hệt. Mục tiêu là chấp nhận hai vé độc lập và giữ đúng EventId đầu tiên cho mỗi vé.

**Tiêu chí nghiệm thu:** `./verify.ps1` trả exit code 0; không sửa `Program.cs` hoặc scripts để lách assertions; giải thích được tại sao các thiết bị gửi biểu diễn khác nhau mà cùng một định danh theo chính sách.
