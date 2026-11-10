# Reference Solution — UNIT-FILEATOMIC-001

## Root cause
`FileMode.Create` trên file đích làm truncate bản báo cáo đã commit **trước khi** dữ liệu mới được ghi xong. Một lỗi I/O ở giữa khiến file đích tồn tại nhưng chứa dữ liệu chưa hoàn chỉnh.

## Cách sửa
Ghi dữ liệu mới vào file tạm cùng thư mục, `Flush(true)` rồi đóng handle; chỉ sau khi ghi thành công mới dùng `File.Move(temp, target, overwrite: true)`. Trong `finally` dọn file tạm khi thất bại. Phép đổi tên cùng filesystem thường cung cấp atomic visibility, khác với ghi trực tiếp.

## Kiểm chứng
`./scripts/verify.ps1` kiểm tra **starter đã sửa**, không chạy solution. Để đối chiếu, dùng `dotnet run --project ./solution/CensusLab.csproj`.

## Giới hạn và trade-offs
Atomic rename/replace không tự chứng minh crash durability trên mọi filesystem, network share hoặc khi mất điện; durability của directory metadata có thể đòi hỏi filesystem-specific sync. Với nhiều writer, cần lock/optimistic versioning. Không giả định thao tác move qua hai volume là atomic. Với file đích nhạy cảm, cân nhắc metadata/ACL preservation, backup và recovery scan cho temp còn sót sau process kill.

## Wrong fixes
- `try/catch` rồi bỏ qua lỗi: che giấu dữ liệu hỏng.
- `Flush(true)` trên file đích: làm dữ liệu hỏng bền vững hơn, không khôi phục bản cũ.
- Viết temp trên volume khác: có thể mất tính atomic khi copy/move.
- Retry ghi trực tiếp: vẫn có cửa sổ file bị truncate, có thể phá hỏng bản cuối cùng khi retry cũng lỗi.
