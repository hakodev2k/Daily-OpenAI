# Senior Challenge — Zero-copy contract

Sau khi verify bản sửa, thiết kế phiên bản transport nhận `ReadOnlyMemory<byte>` thay cho `byte[]` và giữ đúng offset/count từ `TryGetBuffer`. Viết ra lifetime invariant: không dispose/reuse/mutate backing storage trước khi send hoàn tất. So sánh allocation và throughput bằng benchmark với 10.000 frame (không khẳng định hiệu năng khi chưa đo). Đề xuất cách làm khi payload vượt 1 MiB và gateway cần backpressure.
