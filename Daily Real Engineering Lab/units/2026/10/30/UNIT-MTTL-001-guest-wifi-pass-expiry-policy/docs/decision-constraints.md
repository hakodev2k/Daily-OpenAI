# Hợp đồng nghiệp vụ và ràng buộc thiết kế

## Quy mô và hành vi

- Một cơ sở cấp tối đa 100.000 thẻ khách còn tồn tại trong collection; lưu 90 ngày nhật ký quyết định không chứa mật khẩu hoặc token.
- Cổng xác thực .NET xử lý cao điểm 2.000 lượt kiểm tra/giây, mục tiêu p95 dưới 80 ms trong vùng.
- Mỗi thẻ có ExpiresAtUtc. Tại thời điểm nowUtc >= ExpiresAtUtc, yêu cầu phải DENY, không có grace period.
- Thẻ có RevokedAtUtc: kể từ thời điểm thu hồi có hiệu lực, phải DENY. Sau khi ghi nhận thu hồi thành công, SLA áp dụng tại gateway tối đa 5 giây.
- Hệ thống đang lưu thẻ trong MongoDB và có TTL index theo ExpiresAtUtc để giảm dung lượng. Cleanup có thể trễ 0–180 giây trong incident fixture.
- Hai replica gateway cùng phục vụ khách; không thêm dịch vụ trả phí trong hai tuần triển khai. Dữ liệu có thể đi qua cache nhưng phải chứng minh ràng buộc thu hồi.
- Nếu kho xác thực không thể trả lời, chính sách bảo mật mặc định là DENY; gateway phải trả trạng thái lỗi tạm thời riêng với thẻ không hợp lệ để phục vụ vận hành.

## Bài toán quyết định

Thiết kế luồng cấp thẻ, kiểm tra, thu hồi và dọn dữ liệu. Chọn nơi đánh giá hiệu lực truy cập. Đề xuất kiểm soát độ trễ thu hồi giữa hai gateway, phương án khi MongoDB lỗi, và kế hoạch triển khai không gây downtime.

Trong ADR, so sánh ít nhất:
1. Chỉ dựa vào việc bản ghi còn tồn tại.
2. Đọc trạng thái và thời điểm hiệu lực tại mỗi lần kiểm tra.
3. Signed short-lived token kết hợp trạng thái thu hồi hoặc cache.

Nêu chi phí latency, consistency, observability, security và rollback. Phân biệt cleanup retention với authorization. Fixture là mô hình hành vi, không chứng minh SLA hoặc read concern của MongoDB production.
