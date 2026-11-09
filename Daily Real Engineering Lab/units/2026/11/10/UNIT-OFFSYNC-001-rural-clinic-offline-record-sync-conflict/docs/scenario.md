# Scenario và constraints

Phòng khám có 2 tablet A/B và một server. Mỗi tablet có thể chỉnh sửa lượt khám V-204 khi offline. Bản triển khai hiện tại nhận `PUT /visits/V-204` chứa toàn bộ document và `clientUpdatedAt`; server giữ bản có timestamp lớn nhất, bỏ bản còn lại mà không báo conflict. Client clock không được đồng bộ NTP. Cả hai tablet đã tải bản revision 7 trước khi mất kết nối.

Yêu cầu mới: không làm mất ghi chú dị ứng; thao tác đã nhận phải có audit trail; khi hai chỉnh sửa khác field có thể merge nhưng không được tự động merge nếu một bên sửa medication/allergy; request retry không tạo thay đổi hai lần; hệ thống cần hoạt động khi offline tối đa 48 giờ.

Ràng buộc: 300 phòng khám, 2.000 thiết bị, reconnect burst 50 request/giây; server dùng PostgreSQL; không sử dụng dữ liệu bệnh nhân thật; chi phí triển khai phải được so sánh với rủi ro vận hành.

Nhiệm vụ: đưa ra API contract với baseRevision, operationId, kết quả 200/409 và dữ liệu conflict; sequence diagram dạng văn bản; migration từ full-document PUT; chỉ số quan sát và phương án rollback. Không xem lời giải trước khi lập quyết định.
