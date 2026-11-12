# Reference Solution — spoiler

HTTP `Set-Cookie` khác với nhiều header có thể gộp thành danh sách dấu phẩy. Mỗi cookie phải được biểu diễn bằng một field value riêng. Adapter cũ gộp hai giá trị thành một chuỗi; việc chia lại bằng dấu phẩy cũng không an toàn vì thuộc tính `Expires` có thể chứa dấu phẩy.

Bản tham chiếu ở [Program.cs](Program.cs) giữ danh sách giá trị thay vì một chuỗi. Chép nội dung này vào `starter/Program.cs`, sau đó chạy `./verify.ps1`.

**Wrong fixes:** (1) nối bằng dấu chấm phẩy — tạo sai ranh giới cookie; (2) split dấu phẩy tại gateway — phá `Expires=Wed, 21 Oct...`; (3) chỉ giữ cookie đăng nhập — bỏ trạng thái lựa chọn ghế. Trong ASP.NET Core thực tế hãy sử dụng API response headers hỗ trợ nhiều giá trị / cookie append, không tự serialize thành một field.

**Trade-off:** Mô hình `Dictionary<string,string>` đơn giản nhưng không giữ cardinality cho những header không thể coalesce. Mô hình `Dictionary<string,List<string>>` đòi hỏi quy tắc serialize riêng theo từng header. Kiểm thử cần quan sát raw response header fields, không chỉ nội dung chuỗi log.
