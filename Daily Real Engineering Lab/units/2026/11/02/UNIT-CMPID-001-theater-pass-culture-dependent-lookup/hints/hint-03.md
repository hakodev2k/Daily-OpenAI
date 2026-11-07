# Hint 03 — Chọn semantics cho mã máy

Tìm một `StringComparer` dùng so sánh ordinal không phụ thuộc `CultureInfo.CurrentCulture`, nhưng vẫn cho phép ASCII upper/lower case tương đương. Thay comparer ở nơi khởi tạo registry, sau đó verify cả lookup và đăng ký trùng.
