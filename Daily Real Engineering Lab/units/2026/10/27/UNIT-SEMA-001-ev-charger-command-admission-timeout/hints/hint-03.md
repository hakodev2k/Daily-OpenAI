# Hint 03 — Ownership

Kiểm tra giá trị trả về của `SemaphoreSlim.WaitAsync(TimeSpan, CancellationToken)`. Một lần `Release` chỉ hợp lệ khi caller thực sự đã acquire permit. Xác định đường đi không có ownership nhưng vẫn vào callback/finally.
