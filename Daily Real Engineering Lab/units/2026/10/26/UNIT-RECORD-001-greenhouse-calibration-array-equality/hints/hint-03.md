# Hint 03 — Concept direction
Phân biệt shallow record equality với structural equality của array. So sánh nội dung bằng `AsSpan().SequenceEqual` và bảo đảm trạng thái đã áp dụng là bản sao độc lập thay vì giữ reference đến array mutable do caller sở hữu.
