# Before — starter chưa sửa
- `observe` cho thấy cả hai đường gọi đều trả `Accepted=True`.
- Sau reload, fake hiển thị `Approved, Version=8` nhưng production-like store vẫn `Pending, Version=7`.
- `reproduce` exit 0 vì phát hiện đúng sự bất nhất.
- `verify` exit 1 vì ít nhất một contract không được bảo đảm.
