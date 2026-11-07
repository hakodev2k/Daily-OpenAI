# Hint 03 — Tìm hợp đồng options

Phân biệt `IOptions<T>`, `IOptionsSnapshot<T>` và `IOptionsMonitor<T>` khi consumer là singleton. Kiểm tra khi nào `CurrentValue` được đọc và liệu configuration change token có làm invalidation cache options hay không.
