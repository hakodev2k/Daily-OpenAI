# Hint 3

Giữ liveness độc lập. Readiness nên trả non-success trong thời gian warmup chưa hoàn tất và chỉ chuyển sang success khi `WarmupState.IsReady` là `true`.
