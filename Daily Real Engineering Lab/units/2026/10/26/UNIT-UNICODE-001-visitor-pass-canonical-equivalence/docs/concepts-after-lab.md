# Concepts after lab
Unicode có thể biểu diễn cùng một ký tự hiển thị bằng một code point đã ghép hoặc nhiều code point kết hợp. `StringComparer.Ordinal` so sánh các đơn vị mã theo thứ tự, không tự bảo đảm canonical equivalence. `string.Normalize(NormalizationForm.FormC)` tạo dạng chuẩn cho phép so sánh ordinal trên định danh đã chuẩn hóa.

Trong hệ thống thật, canonicalization cần là một phần của **versioned identity contract**. Nếu DB unique constraint dùng khóa raw còn service dùng khóa chuẩn hóa, duplicate vẫn lọt qua race condition. Không nhầm canonical equivalence với case folding, culture-aware comparison hay confusable detection.
