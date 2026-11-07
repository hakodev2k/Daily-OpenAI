# Incident: quét tài liệu hồ sơ bảo hiểm

Ứng dụng nhận yêu cầu validate tài liệu, chuyển sang scanner của đối tác rồi chờ receipt. Có hai nguồn hủy hợp lệ: `RequestAborted` khi client rời đi và `ApplicationStopping` khi host shutdown. Nếu một trong hai phát tín hiệu, scan đang chờ phải kết thúc sớm; nếu không có tín hiệu, scan phải trả receipt bình thường.

Dữ liệu giả lập: trong 20 phút sau rollout, hệ thống ghi 37 request bị client hủy, nhưng số scan đang chờ không giảm tương ứng. Partner health check bình thường, không có HTTP 5xx tăng đáng kể. Một kỹ sư đề xuất tăng timeout; kỹ sư khác nghi ngờ scanner không hỗ trợ cancellation.

## Hợp đồng cần bảo toàn
- Không thay đổi `IExternalScanner` hay các record công khai.
- Cancellation từ caller **hoặc** host đều có thể dừng scan.
- Không hủy nhầm một scan đang hoạt động bình thường.
- Không bỏ rơi task hoặc giữ registration sau khi thao tác đã hoàn thành.
- Fake scanner không gọi network; test độc lập, không cần sleep theo latency production.

## Gợi ý điều tra
So sánh trạng thái token ở boundary với token thực sự được scanner nhận. Phân biệt `task còn pending` với `task đã kết thúc do cancellation`. Kiểm tra lifetime tài nguyên được tạo trong service so với lifetime thao tác downstream.
