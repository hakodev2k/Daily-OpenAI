# Hint 3
`CancellationTokenSource` đã cancel không thể reset bằng `CancelAfter`. Tạo CTS mới cho mỗi cycle, liên kết với shutdown, dispose sau khi cycle kết thúc.
