# Incident timeline — dữ liệu mô phỏng

| Thời điểm | Sự kiện | Dấu vết |
|---|---|---|
| 09:00 | Release candidate B được phê duyệt | Pipeline lưu mã artifact B trong change record |
| 09:02 | Cập nhật release spec | Cả hai worker đều hiện cùng image reference trong dashboard |
| 09:03 | node-east được đưa vào service | Ready=True; thuật toán báo revision A; không có network pull |
| 09:04 | node-west được đưa vào service | Ready=True; thuật toán báo revision B; có registry resolution |
| 09:05 | 1.000 request thử nghiệm | 100% HTTP 200; 51% trả route-score A, 49% trả route-score B |
| 09:06 | Đo hạ tầng | CPU 38–46%; Redis p95 2–3 ms; không có restart loop |
| 09:08 | Thử rollback thủ công | Nhãn release thay đổi trong ticket, chưa chứng minh revision thực chạy |

Đây là fixture tổng hợp, không phải production telemetry thật. Hãy ghi giả thuyết về code, cache node, artifact registry, health probe và request routing; chọn phép thử có sức phân biệt cao nhất.
