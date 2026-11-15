# Reference solution (spoiler)

Nguyên nhân: `ZipArchiveEntry.FullName` là dữ liệu không tin cậy; `Path.Combine` không đảm bảo kết quả nằm trong thư mục extract. Entry `../outside.txt` thoát ra thư mục cha.

Giải pháp: canonicalize bằng `Path.GetFullPath`, so sánh với extraction root đã canonicalize cộng dấu phân cách thư mục; từ chối entry không nằm dưới root trước khi tạo file. Bản tham chiếu nằm ở `solution/Program.cs`. Với Linux, nên dùng so sánh `StringComparison.Ordinal`; trên Windows dùng `OrdinalIgnoreCase`. Bản tham chiếu dùng `OrdinalIgnoreCase` để tương thích Windows; khi triển khai đa nền tảng phải điều chỉnh theo filesystem.

Không chỉ lọc chuỗi `..` vì đường dẫn tuyệt đối, các cách biểu diễn tương đương và separator có thể vượt qua lọc thô. Trong production, còn cần giới hạn kích thước giải nén, số entry, overwrite policy, symlink/reparse-point policy, quota, và xử lý TOCTOU trên thư mục có thể bị bên khác sửa.

## Trade-offs
Từ chối toàn bộ archive khi có entry không hợp lệ (fail closed) là lựa chọn an toàn hơn cho dữ liệu nhạy cảm; bản demo chỉ bỏ qua entry bị từ chối để quan sát được file hợp lệ. Dùng `ZipFile.ExtractToDirectory` của .NET có cơ chế chặn escape path, nhưng vẫn cần quy định overwrite, giới hạn tài nguyên và chính sách an toàn file.
