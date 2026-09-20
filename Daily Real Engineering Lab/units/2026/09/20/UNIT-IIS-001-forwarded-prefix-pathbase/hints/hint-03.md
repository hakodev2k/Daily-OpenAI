# Hint 3

Nếu proxy strip `/staff` trước khi forward, application cần một trusted signal để khôi phục prefix đó thành `PathBase` trước routing/link generation. Trong production, chỉ tin forwarded metadata từ proxy đã cấu hình tin cậy.