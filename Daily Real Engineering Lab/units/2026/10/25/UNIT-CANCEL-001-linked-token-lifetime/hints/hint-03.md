# Hint 3 — Lifetime của nguồn hủy liên kết

Kiểm tra `CancellationTokenSource.CreateLinkedTokenSource` và thời điểm `Dispose` chạy trong method trả về task. Một linked CTS phải tồn tại đủ lâu để chuyển tiếp các tín hiệu cancellation trong suốt operation. Đối chiếu hành vi của method `async` thực sự chờ task trước khi thoát scope.
