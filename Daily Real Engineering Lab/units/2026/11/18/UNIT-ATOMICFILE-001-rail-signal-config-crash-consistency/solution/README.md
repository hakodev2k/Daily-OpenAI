# Lời giải tham khảo — chỉ đọc sau khi tự verify

## Root cause
`File.WriteAllText(path, ...)` mở file đích theo cách thay thế nội dung ngay; lỗi xảy ra sau khi ghi một phần khiến snapshot cũ đã bị mất. JSON mới chưa hoàn chỉnh không thể parse.

## Cách sửa
Ghi đầy đủ vào file tạm **cùng thư mục**, sau đó dùng `File.Move(temp, path, overwrite: true)` để đổi tên khi hoàn tất. Trong bài tập, lỗi được tiêm trước thao tác đổi tên; `finally` xóa file tạm. Đối chiếu `solution/Program.cs` với `starter/Program.cs`.

## Kiểm chứng
`dotnet run --project solution/SignalConfigLab.csproj -- verify` phải PASS. `scripts/verify.ps1` chỉ kiểm tra code người học ở starter.

## Giới hạn production
Atomic rename trên cùng filesystem không đồng nghĩa bảo đảm dữ liệu bền vững qua mất điện; cần xem xét flush/fsync, filesystem, antivirus, quyền truy cập, concurrent writers và chiến lược versioning/backup. Không dùng giả định atomicity xuyên volume hoặc network share. Có thể cần serialization lock và durability protocol theo yêu cầu RPO.
