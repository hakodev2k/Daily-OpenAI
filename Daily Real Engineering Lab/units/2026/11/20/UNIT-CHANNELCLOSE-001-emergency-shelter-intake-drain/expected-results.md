# Before
- `reproduce.ps1` thành công khi xác nhận lỗi gốc.
- `--check` của starter trả exit code 1.
- `accepted=2; processed=1; missing=south-205`; event `south write rejected`.

# After
- `verify.ps1` chạy trên starter đã sửa, trả thành công.
- `accepted=2; processed=2; missing=`, không có write rejected.
- Không phụ thuộc timing máy vì fixture dùng TaskCompletionSource.

# Troubleshooting
Kiểm tra `dotnet --version` có SDK 8; khôi phục starter gốc nếu reproduce không còn thất bại; script dùng `$PSScriptRoot`.
