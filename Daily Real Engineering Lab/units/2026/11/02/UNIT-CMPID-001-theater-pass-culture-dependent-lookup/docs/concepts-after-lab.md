# Concepts after lab — Equality là một phần của data contract

- **Culture-sensitive comparison** phục vụ từ ngữ tự nhiên và quy tắc ngôn ngữ; không mặc định phù hợp với protocol identifier.
- **Ordinal comparison** đánh giá code points theo quy tắc ordinal, độc lập với locale hiện hành. `OrdinalIgnoreCase` phù hợp khi identifier được định nghĩa ASCII case-insensitive.
- **Dictionary equality và hashing** phải dùng cùng một comparer để lookup, `TryAdd` và duplicate detection nhất quán.
- **Unicode lookalikes** không tự động là cùng một ký tự. Nếu sản phẩm cho phép Unicode identifiers, phải có chính sách normalization, confusable handling và security review riêng.
- **Identity contract** cần được test trên ít nhất hai locale, không chỉ trên máy developer.

Đọc sau khi hoàn thành investigation; chi tiết fix nằm trong `solution/`.
