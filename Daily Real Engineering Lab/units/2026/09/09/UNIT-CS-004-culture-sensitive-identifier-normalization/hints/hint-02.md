# Hint 2

Một cách an toàn là tránh normalize thủ công và để `Dictionary` dùng `StringComparer.OrdinalIgnoreCase` nếu domain coi identifier là case-insensitive theo ordinal semantics.
