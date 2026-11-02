# Hint 03 — Chính sách xử lý

Generic Host có chính sách khi `BackgroundService.ExecuteAsync` kết thúc bằng unhandled exception. Hãy phân loại lỗi gateway nào có thể cô lập ở cấp item, lưu dấu thất bại để xử lý lại, và để lỗi không mong đợi tiếp tục nổi lên.
