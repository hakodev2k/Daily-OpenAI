# Senior challenge — capacity ownership under shutdown
1. Thiết kế thêm case shutdown: host yêu cầu hủy A khi A đã acquire; A phải giải phóng permit chính xác một lần, không rò rỉ capacity.
2. Tạo stress test với 100 request, trong đó một số request bị hủy khi đang chờ; invariant `active <= maxWorkers` phải luôn đúng mà không cần `Task.Delay` để tạo ordering.
3. Nếu hệ thống scale lên 3 replica, hãy phân biệt per-node limit với global limit và so sánh semaphore local, bounded Channel, broker-based workers và distributed coordinator.
4. Đề xuất metrics có cardinality hữu hạn, cảnh báo capacity violation và cách graceful shutdown không mất work đã nhận.
5. Phân tích trade-off fairness, starvation và throughput; không mặc định rằng `SemaphoreSlim` bảo đảm FIFO.
