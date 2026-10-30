# Concepts after lab — Value type, boxing và shared state

Trong C#, struct có value semantics. Khi một giá trị struct được chuyển thành biến interface mà nó implement, CLR boxing một bản sao của giá trị thành object. Worker gọi phương thức qua interface trên bản boxed đó; biến struct ban đầu không được cập nhật.

Điều này khác với class: nhiều tham chiếu có thể trỏ đến cùng một instance. Với bộ đếm mutable cần được nhiều thành phần quan sát, phải làm rõ hợp đồng sở hữu trạng thái và thời gian sống. Không phải cứ dùng class là tự động thread-safe: nếu nhiều worker cập nhật đồng thời, cần Interlocked, lock hoặc thiết kế immutable aggregation tùy yêu cầu.

Các lựa chọn khác: trả về số lượng từ worker và dùng kết quả đó làm nguồn sự thật, hoặc dùng bộ đếm class có scope theo ca. Tránh dựa vào bản sao struct có mutable state để chia sẻ giữa các thành phần.

Tài liệu chính thức:
- https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-types
- https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/types/boxing-and-unboxing
