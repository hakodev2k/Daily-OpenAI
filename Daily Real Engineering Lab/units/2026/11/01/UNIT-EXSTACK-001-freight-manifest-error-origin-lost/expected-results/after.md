# After — learner-modified starter

- `./scripts/verify.ps1`: exit 0, `VERIFIED`.
- Stack trace chứa cả `SourceParser.Parse` và `ImportCoordinator.Import`.
- `FormatException` vẫn được truyền lên; bản ghi hợp lệ vẫn trả đúng dữ liệu; audit vẫn chỉ ghi một rejection.
- `./scripts/reproduce.ps1`: exit khác 0 vì triệu chứng cũ đã biến mất.

Nếu verify thất bại, xem phần `CHECK FAILED` hoặc `VERIFY FAILED`, kiểm tra số audit event và exception type trước khi xem reference solution.
