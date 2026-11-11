# Scenario & Constraints

Cơ quan quản lý lưu vực cần phát cảnh báo cho khu vực dân cư. Ngưỡng: bất kỳ trạm nào đạt >= 500 cm trong cửa sổ event-time 09:00–09:10 thì không được phát thông báo CLEAR cho cửa sổ đó. Trạng thái CLEAR là thông tin có tác động vận hành; cập nhật có thể bị người dân đọc lại.

Hiện trạng: worker mỗi 10 phút tổng hợp các sự kiện đã *nhận* đến thời điểm chạy, đóng cửa sổ theo đồng hồ server, và gửi CLEAR nếu không thấy mẫu vượt ngưỡng. Không có bản sửa đổi thông báo.

Dữ liệu CSV chứa receivedAt (UTC), measuredAt (UTC), stationId, readingId, revision, levelCm. Revision lớn hơn thay thế revision cũ của cùng readingId; cùng revision lặp lại không tạo phép đo mới. Một record được phép tới sau tối đa 8 phút trong SLA bình thường; 0.5% vượt 8 phút; mạng có thể offline 25 phút. Không được loại bỏ im lặng late data.

Ràng buộc: 12 trạm, 1 record/30 giây/trạm, tối đa burst 100 lần khi reconnect; vận hành 2 kỹ sư, ngân sách bổ sung <= $250/tháng, RTO 30 phút, RPO 5 phút; cảnh báo ban đầu phải có trong 2 phút từ lúc hệ thống nhận mẫu vượt ngưỡng. Mọi thông báo phải có stable alertId, windowId, revision, trạng thái provisional/final, issuedAt và nguồn dữ liệu.

Các phương án cần so sánh: A) processing-time + correction manual; B) event-time watermark + late correction; C) chờ 25 phút trước mọi cảnh báo. Không được giả định event-time ordering hoặc exactly-once delivery.

Requirement change: tháng sau 2 trạm chỉ đồng bộ mỗi 45 phút; ngân sách không tăng. Nêu cách thay đổi policy mà không làm mất khả năng audit và không âm thầm giảm SLA.