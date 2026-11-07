# Senior challenge (optional)

Renderer thật có thể ghi 2 GiB log và nhận CancellationToken từ request. Thiết kế bounded streaming cho stdout/stderr, giới hạn byte, giữ tail log, phân biệt caller cancellation và timeout nội bộ. Nêu chính sách kill/cleanup Windows và Linux, race giữa process exit và cancellation, kiểm thử nonzero exit sau khi stderr đã đầy.
