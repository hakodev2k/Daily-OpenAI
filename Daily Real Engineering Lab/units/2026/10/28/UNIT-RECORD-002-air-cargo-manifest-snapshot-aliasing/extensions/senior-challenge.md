# Senior Challenge — concurrency và scale

Giả sử 16 máy scan gọi `ScanBag` đồng thời, mỗi chuyến bay có 100.000 bag tags và phải xuất bản snapshot audit trong 200 ms. Bản reference single-thread không bảo đảm atomicity giữa read-copy-write. Hãy đề xuất và benchmark ít nhất hai lựa chọn: lock per flight, immutable collection + atomic compare-and-swap, hoặc append-only event log với snapshot checkpoint. Đánh giá lost update, memory allocation, throughput, retention, và cách test race deterministically. Không thay đổi yêu cầu audit immutability.
