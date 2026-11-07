# Hint 3 — Hợp đồng tiêu thụ output

Đọc tài liệu ProcessStartInfo.RedirectStandardError về deadlock. Kiểm tra cả hai redirected stream có được drain đồng thời trước khi chờ process exit không. Một Task chạy thao tác blocking không tự động làm các stream độc lập.
