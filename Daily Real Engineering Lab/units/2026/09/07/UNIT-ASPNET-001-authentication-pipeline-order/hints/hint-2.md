# Hint 2

Đặt breakpoint hoặc log ngay trước access gate và ngay trước endpoint. So sánh `HttpContext.User.Identity?.IsAuthenticated` tại hai vị trí. Hỏi: identity đã được thiết lập trước khi access decision được đưa ra chưa?
