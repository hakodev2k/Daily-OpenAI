# Incident snapshot — ca 10:00–10:15 UTC

Dữ liệu được trích từ output giả lập trước khi có thay đổi. \`Delta=+1\` nghĩa là một lượt vào, \`Delta=-1\` là một lượt ra.

| Window UTC | Dashboard net flow | Reconciliation from raw gate events |
|---|---:|---:|
| 10:00–10:05 | +1 | +1 |
| 10:05–10:10 | +1 | 0 |
| 10:10–10:15 | 0 | +1 |
| **Tổng** | **+2** | **+2** |

Các bằng chứng khác: CPU của worker 18–25%; 0 retry; 0 exception; 6 event IDs phân biệt; cả sáu event xuất hiện trong input. Không có kết luận root cause nào được xác nhận trước khi xem code và timestamps.

Đầu mối điều tra: xuất bảng EventId, thời gian xảy ra, thời gian hệ thống nhận và khung mà dashboard ghi nhận; so sánh với hợp đồng cửa sổ 5 phút.
