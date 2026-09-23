# Evidence

Inventory ban đầu của SKU mẫu là 10 units. Hai request độc lập cùng yêu cầu reserve 6 units.

Timeline quan sát được:

| Step | Request A | Request B |
|---|---|---|
| 1 | đọc available = 10 | |
| 2 | validation pass | |
| 3 | | đọc available = 10 |
| 4 | | validation pass |
| 5 | ghi reservation 6 | |
| 6 | | ghi reservation 6 |

Cả hai request đều có thể báo thành công dù tổng reserved vượt inventory ban đầu.

## Noise / secondary observations

- CPU application bình thường.
- Không có network timeout trong incident window.
- Hai request có correlation ID khác nhau.
- Application chạy nhiều instances.

Hãy phân biệt evidence liên quan trực tiếp tới invariant với evidence chỉ giúp loại trừ hypothesis khác.