# Hint 03 — Concept direction

Tách bước kiểm tra cấu trúc JSON object khỏi bước materialize DTO. Có thể duyệt `JsonElement.EnumerateObject()` và dùng một comparer phù hợp để phát hiện tên logic xuất hiện nhiều lần trước khi deserialize.
