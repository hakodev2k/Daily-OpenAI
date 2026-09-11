# Hint 03

Nếu duplicate là một outcome nghiệp vụ dự kiến, hãy cân nhắc xử lý conflict ngay trong SQL command thay vì dùng exception như control flow. PostgreSQL có syntax cho phép một `INSERT` tự xử lý conflict mà không đưa transaction vào failed state.
