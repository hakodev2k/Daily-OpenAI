# Hint 3
Đọc semantics của BoundedChannelFullMode và ChannelWriter.TryWrite so với WriteAsync. Với yêu cầu lossless, producer cần chờ khi đầy và consumer phải chạy song song; lỗi sink phải unblock producer đang chờ.
