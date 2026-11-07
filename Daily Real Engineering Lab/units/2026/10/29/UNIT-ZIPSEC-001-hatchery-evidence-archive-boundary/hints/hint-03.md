# Hint 03
Path.Combine không bảo đảm containment. Xem Path.GetFullPath, ranh giới separator, StringComparison phù hợp OS và kiểm tra toàn bộ entry trước khi trích xuất. Đặt quy tắc rõ cho backslash trong ZIP.
