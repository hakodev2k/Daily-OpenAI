# Reference solution — UNIT-CURSORPAGE-001

Root cause: offset paging sử dụng vị trí thay vì identity; insert trước offset làm dịch trang. API thay bằng cursor `beforeExclusive`, `snapshotMaxInclusive`, `pageSize`. Query MongoDB tương ứng `{ sequence: { $lte: snapshotMax, $lt: lastSeen } }` (khi có cursor, kết hợp điều kiện theo giá trị nhỏ hơn), `sort({sequence:-1}).limit(pageSize)`. Tạo unique index cho sequence; nếu dùng `createdAt` thì cần tie-breaker `_id` và lexicographic predicate. Snapshot upper bound ngăn item mới chen vào, nhưng không cung cấp snapshot isolation đối với delete/update; cần snapshot read/retention nếu yêu cầu duyệt tập bất biến. Cursor nên được ký, kiểm tra tenant và expiry, tránh user sửa boundary.

**Trade-off:** keyset không nhảy trực tiếp tới trang N; offset phù hợp dataset bất biến nhỏ, nhưng không bảo đảm duyệt ổn định khi có concurrent writes. Không chỉ tăng page size hoặc thêm retry.

Sau khi thay `starter/AuditPaging.cs` bằng API trong `solution/AuditPaging.cs`, chạy `pwsh ./scripts/verify.ps1`. Lưu ý starter `Reproduce.cs` dùng contract cũ nên `run.ps1` dành cho baseline trước refactor; verify biên dịch workspace riêng theo contract mới.