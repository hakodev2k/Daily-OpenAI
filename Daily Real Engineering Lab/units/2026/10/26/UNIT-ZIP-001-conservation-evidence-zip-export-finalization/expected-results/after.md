# After — starter đã sửa

Chạy `./scripts/verify.ps1`:
- Ba lần export đều có `status=Valid` và nội dung đúng của hai entry.
- Gói rỗng có `status=Valid` với 0 entry.
- Dòng cuối `VERIFIED: ZIP integrity and content contract`; exit code 0.
- `verify.ps1` luôn chạy **starter đã sửa**, không chạy reference solution.
- Tự kiểm tra thêm với consumer ZIP bên ngoài nếu cần, nhưng không thay thế kiểm thử hợp đồng bằng mắt thường.
