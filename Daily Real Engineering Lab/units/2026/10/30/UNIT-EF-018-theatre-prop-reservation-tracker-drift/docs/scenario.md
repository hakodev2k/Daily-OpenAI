# Scenario & Acceptance Contract

Một đơn vị đạo cụ chỉ có một kho vật lý, biểu diễn bằng `PropStock.Available`. Mỗi lượt chạy tạo một SQLite in-memory database mới, vì vậy kết quả độc lập và có thể lặp lại.

| Opening | Stage reserved | Counter sold | Expected persisted | Expected counter result |
|---:|---:|---:|---:|---|
| 10 | 2 | 1 | 7 | Accepted |
| 12 | 3 | 4 | 5 | Accepted |
| 3 | 2 | 2 | 1 | Rejected |

Giả định: hai nghiệp vụ diễn ra theo thứ tự, không có request bên ngoài xen vào. Đây là bước điều tra tính nhất quán **trong một request**. Extension: khi có nhiều request đồng thời, cần thiết kế điều kiện cập nhật nguyên tử hoặc optimistic concurrency, không suy diễn rằng một bản sửa nội bộ đã giải quyết race giữa nhiều request.

Acceptance: cả ba fixture chạy qua `verify.ps1`; không làm âm tồn kho; không ghi đè phân bổ sân khấu; không chấp nhận thuê khi thiếu hàng. Không chỉnh sửa expected fixture để làm test xanh.
