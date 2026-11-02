# Investigation guide

1. Lưu output của `scripts/reproduce.ps1`, phân loại từng failure.
2. Breakpoint tại `Publish`, `GetHistory`, và các phép gán trong harness.
3. So sánh giả thuyết: ghi đè revision, filter sai, hoặc shared object graph.
4. Dùng `ReferenceEquals` để quan sát object identity ở từng tầng, nhưng đánh giá fix bằng behavior contract.
5. Thử mutation cả ở đầu vào lẫn đầu ra; bảo vệ public API và chạy verify trên `starter/`.
