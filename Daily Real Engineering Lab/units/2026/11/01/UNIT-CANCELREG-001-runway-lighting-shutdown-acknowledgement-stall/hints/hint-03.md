# Hint 3
Callback đăng ký bằng `CancellationToken.Register` có thể chạy đồng bộ trong `Cancel()`. Đừng đợi downstream trong callback; phát tín hiệu nhanh và dùng Task cleanup có ownership rõ ràng, không `async void`.
