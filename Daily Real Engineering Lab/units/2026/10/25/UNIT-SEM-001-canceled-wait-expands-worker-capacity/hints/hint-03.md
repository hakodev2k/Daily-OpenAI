# Hint 03 — Cleanup boundary
`WaitAsync` có thể ném `OperationCanceledException` **trước khi permit được cấp**. Đặt phần cleanup chỉ trong phạm vi được thực thi sau khi acquire thành công. Đừng chỉ tăng `maxCount` hoặc bắt exception rồi tiếp tục.
