# Incident evidence — dữ liệu mô phỏng, không phải log từ một lần chạy thật

## Báo cáo từ đội vận hành
- 02:00: Batch cập nhật bảng giờ xe từ North, West, East.
- 02:01: North ghi nhận đã trả về 42 hàng.
- 02:01: West có TimeoutException.
- 02:01: East có InvalidOperationException do cấu trúc feed thay đổi.
- 02:02: Dashboard chỉ có một mục lỗi cấp batch, không có mục thành công.

## Bằng chứng đối chiếu
| Nguồn | Telemetry giả lập tại adapter | Báo cáo cấp batch hiện tại |
|---|---|---|
| North | Success, 42 rows | Không thấy |
| West | TimeoutException | Không có mục riêng |
| East | InvalidOperationException | Không có mục riêng |
| Tổng | 1 thành công, 2 lỗi | 0 thành công, 1 lỗi generic |

## Nhiễu điều tra
- CPU máy worker ở mức thấp; không có dấu hiệu ThreadPool saturation.
- DB health check vẫn thành công.
- Có một HTTP 404 không liên quan từ trang quản trị.

## Nhiệm vụ
Từ evidence và kết quả của **./reproduce.ps1**, đưa ra ít nhất ba giả thuyết có thể kiểm chứng. Xác định điểm mất thông tin bằng cách so sánh trạng thái từng operation với báo cáo trả về. Đừng coi thông số telemetry giả lập là số đo đã chạy thực tế.
