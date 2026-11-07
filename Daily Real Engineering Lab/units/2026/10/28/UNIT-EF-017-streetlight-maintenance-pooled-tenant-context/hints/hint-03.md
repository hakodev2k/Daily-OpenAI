# Hint 03 — Cơ chế
Global query filter dùng property TenantId trên context. Context pooling có thể tái sử dụng cùng instance; property ứng dụng tự định nghĩa không mặc nhiên được reset. Hãy đảm bảo tenant identity của **mỗi lần borrow** được gán trước khi thực hiện truy vấn, rồi chạy cả tenant có dữ liệu và tenant rỗng.
