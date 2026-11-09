# Evidence — replay có kiểm soát

Tất cả số liệu bên dưới là **synthetic** để phục vụ quyết định, không phải đo production.

## Giới hạn
- Gateway hiện tại: **global fixed-window 100 requests/10 giây** trên tất cả tenant.
- Một request được tính 1 token. 429 không thực thi downstream.
- Capacity downstream an toàn đã đo bằng load test: **120 requests/10 giây**; không được thiết kế vượt mức này.
- Mục tiêu: phòng khám khẩn cấp A có ít nhất **30 requests/10 giây** được nhận khi gửi đủ; bulk tenant B có best-effort; tenant C là clinic thường.
- Trong mỗi cửa sổ, gateway hiện xử lý theo thứ tự B → C → A (đã quan sát ở burst), không có retry trong replay.
- Nếu thiếu capacity, request đến sau bị 429. Không carry over giữa các cửa sổ ở baseline.

| Window | A urgent | B bulk | C clinic | Total |
|---|---:|---:|---:|---:|
| W1 | 30 | 20 | 20 | 70 |
| W2 | 30 | 95 | 15 | 140 |
| W3 | 30 | 110 | 20 | 160 |
| W4 | 10 | 25 | 20 | 55 |

## Baseline: global fixed-window
Với thứ tự B → C → A, hãy tính accepted/rejected cho từng tenant trong W1–W4. Tìm cửa sổ có tổng throughput đạt quota nhưng mục tiêu của A thất bại.

## Tín hiệu bổ sung
- CPU p95 58%, DB pool active 64% tại W2; không chứng minh backend có thể nhận vô hạn.
- Một đợt 429 đã khiến client B retry đồng thời sau 1 giây; không tính vào bảng trên, nhưng phải xem xét trong thiết kế.
- Hệ thống có hai gateway replicas; request của một tenant có thể đến cả hai.
- 429 response hiện thiếu Retry-After và không có reason label.
- Chi phí thêm Redis được ước lượng 65 USD/tháng, vận hành có thể hỗ trợ; không bắt buộc dùng Redis.
