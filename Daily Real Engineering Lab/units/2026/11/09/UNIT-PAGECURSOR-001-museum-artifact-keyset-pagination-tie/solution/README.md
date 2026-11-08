# Reference solution

## Root cause
`ORDER BY UpdatedAt, Id` nhưng cursor chỉ giữ UpdatedAt. Khi trang đầu kết thúc tại Id=11, điều kiện `UpdatedAt > cursor.UpdatedAt` loại Id=12 có cùng timestamp.

## Fix
Cursor chứa cả timestamp và unique Id. Điều kiện: `UpdatedAt > @time OR (UpdatedAt = @time AND Id > @id)`, cùng `ORDER BY UpdatedAt ASC, Id ASC`. Xem `solution/Program.cs`.

## Verify
`dotnet run --project solution/ExportLab.csproj -- --verify` phải xuất 10,11,12,13,14, exit 0. Sửa starter rồi chạy `./verify.ps1`.

## Trade-offs
Offset pagination dễ dùng nhưng không ổn định khi ghi đồng thời. Composite keyset có thứ tự ổn định nhưng không thay thế snapshot isolation/high-water mark khi dữ liệu thay đổi giữa các trang.

## Wrong fixes
Tăng page size chỉ che lỗi; dùng `>=` có thể lặp vô hạn; `Distinct` không khôi phục bản ghi thiếu; chỉ thêm `ThenBy(Id)` mà không sửa cursor vẫn sai.
