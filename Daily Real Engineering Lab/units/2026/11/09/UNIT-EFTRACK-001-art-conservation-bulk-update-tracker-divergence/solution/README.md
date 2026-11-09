# Reference Solution — Spoiler

## Root cause
`ExecuteUpdateAsync` thực hiện SQL trực tiếp và không đồng bộ giá trị mới vào ChangeTracker. Entity `artifact` vẫn có `Priority=1`. Dòng `Entry(artifact).State = EntityState.Modified` đánh dấu **mọi** scalar property cần ghi, nên `SaveChangesAsync` đưa Priority cũ trở lại database. Transaction riêng lẻ không giải quyết stale tracked snapshot.

## Minimal fix
Thay:
```csharp
db.Entry(artifact).State = EntityState.Modified;
```
bằng:
```csharp
db.Entry(artifact).Property(x => x.Note).IsModified = true;
```
Với tracked entity, thay đổi `Note` đã được phát hiện tự động; có thể xóa hẳn dòng gán State. Giữ các trường hợp sửa có chủ đích rõ ràng.

## Production option
Dùng DbContext ngắn vòng đời, phân tách set-based update khỏi read-modify-write; khi cần đọc giá trị mới trong cùng context, gọi `Entry(artifact).ReloadAsync()` **trước khi** chỉnh sửa note. Reload sau khi chỉnh note sẽ làm mất thay đổi chưa lưu. Với nhiều người sửa cùng lúc, sử dụng concurrency token/version và xử lý conflict; SQLite demo không tự cung cấp SQL Server rowversion.

## Wrong fixes
- `AsNoTracking()` chỉ trên truy vấn sau bulk không sửa snapshot đã tracked.
- Gói cả hai thao tác trong transaction không làm ChangeTracker tự refresh.
- Tắt verify hoặc đổi expected Priority thành 1 che mất lỗi.
- Reload sau khi sửa note sẽ bỏ mất note.

## Verification
Chạy `scripts/reproduce.ps1` trên bản gốc, sau đó chỉnh `starter/Program.cs`, chạy `scripts/verify.ps1`. Đối chiếu cả priority lẫn note, không chỉ return count của ExecuteUpdate. Trong môi trường thật, bổ sung integration test với provider production và concurrent writer.

## Trade-offs
Property-level update tối thiểu và ít ảnh hưởng; reload cho dữ liệu mới nhưng cần thêm round trip; DbContext isolation giúp dễ reasoning nhưng phải xử lý concurrency và transaction rõ ràng.
