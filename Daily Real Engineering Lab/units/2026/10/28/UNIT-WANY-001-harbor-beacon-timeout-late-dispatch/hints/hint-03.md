# Hint 03 — Hướng cơ chế
`Task.WhenAny` chỉ xác định task kết thúc trước; nó không tự hủy task còn lại. Kiểm tra khả năng dùng `CancellationTokenSource.CreateLinkedTokenSource`, truyền token xuống transport, và **quan sát completion** của tác vụ trước khi trả timeout. Cân nhắc cả trường hợp transport không hỗ trợ cancellation.
