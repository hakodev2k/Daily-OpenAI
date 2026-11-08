# Scenario: Export report R-2408

Một job export có thể được xử lý lại sau khi lease hết hạn. Worker A nhận lease lúc 09:00:00, deadline 09:00:30. Worker B nhận lease mới lúc 09:00:31. A đã tính xong nhưng tạm ngưng trước khi ghi; B tính lại dữ liệu mới hơn.

**Chính sách hiện tại:** worker kiểm tra mình còn lease khi bắt đầu xử lý; cuối pipeline ghi file `reports/R-2408.json` bằng thao tác replace rồi đặt `status=COMPLETED`. Storage không biết worker nào được cấp quyền gần nhất.

**Ràng buộc:** (1) không thể giả định worker bị mất mạng sẽ dừng ngay; (2) lease có thể hết hạn trong khi đang tính; (3) thao tác ghi kết quả và cập nhật trạng thái phải có hợp đồng nhất quán; (4) retry không được phá hủy kết quả hợp lệ; (5) không sử dụng global mutex trong một process làm bảo đảm phân tán.

**Đầu ra mong muốn:** bản mới hơn của job thắng theo quyền sở hữu hợp lệ, không chỉ theo thứ tự đến storage. Giải thích rõ linearization point của commit, token nào được kiểm tra, và cách xử lý khi object store không hỗ trợ transaction chung với database.

## Replay
Dựa vào `../evidence/timeline.csv`, điền bảng gồm: event, current owner, lease generation, write accepted/rejected, output version. Đầu tiên chạy theo chính sách hiện tại, sau đó replay thiết kế của bạn.