# Concepts after lab — đọc sau khi hoàn thành
Hash-based collection xác định vị trí phần tử dựa trên hash code tại thời điểm thêm. Nếu equality/hash code phụ thuộc thuộc tính mutable và thuộc tính đó thay đổi, object có thể nằm trong bucket cũ dù lookup bằng giá trị mới đi vào bucket khác. Điều này phá `Contains`, `Remove`, uniqueness.

Trong registry nghiệp vụ, hãy phân biệt *identity ổn định* với *mã có thể sửa*. Một `Dictionary<string, T>` sử dụng string key và thao tác đổi key có kiểm tra xung đột có thể phù hợp với một registry nhỏ. Ở hệ thống multi-thread hoặc đa instance, cần bảo vệ tính atomic bằng synchronization/transaction và unique constraint tại nguồn dữ liệu.

Không coi đổi `HashSet` thành `List` là giải pháp tự động: lookup tuyến tính và vẫn cần chính sách chống trùng/rename nhất quán.
