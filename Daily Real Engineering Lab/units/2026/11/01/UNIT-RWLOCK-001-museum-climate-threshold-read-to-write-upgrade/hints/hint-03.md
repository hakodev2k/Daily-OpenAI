# Hint 03 — Khái niệm
Tìm hiểu `ReaderWriterLockSlim.EnterUpgradeableReadLock`. Khi chuyển từ pha đọc nhanh sang pha có thể ghi, phải kiểm tra lại dữ liệu sau khi giành quyền thích hợp để tránh tạo trùng. Không giữ upgradeable lock cho mọi cache hit nếu không cần thiết.
