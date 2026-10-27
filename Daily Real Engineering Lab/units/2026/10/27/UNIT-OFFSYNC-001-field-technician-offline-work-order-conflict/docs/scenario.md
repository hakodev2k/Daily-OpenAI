# Scenario & acceptance contract

**Bối cảnh:** Đội bảo trì nước sạch cập nhật một Work Order từ hai tablet A và B. Mỗi tablet có thể offline 72 giờ. Đồng hồ thiết bị không đáng tin cậy và các thay đổi được gửi lại theo thứ tự bất kỳ.

**Quy tắc nghiệp vụ:**

- Hai thay đổi trên **hai field độc lập** phải có thể được giữ lại dù đến không đúng thứ tự thời gian.
- Hai thay đổi cạnh tranh trên **cùng field** từ cùng một base field revision không được tự ghi đè âm thầm.
- Một change ID được retry với payload giống hệt phải được nhận diện là cùng thao tác, không tăng revision.
- Cùng change ID với payload khác phải bị từ chối; field không tồn tại không được tự tạo.
- ClientTime chỉ phục vụ điều tra/telemetry, không được dùng làm nguồn sự thật về thứ tự ghi.
- Trạng thái kết quả phải phân biệt Applied, Conflict và AlreadyApplied.
- Không sửa các assertions của starter/Program.cs để làm test xanh.

**Dữ liệu:** Work Order ban đầu có Status=Assigned và TechnicianNote=None, mỗi field revision=0.

**Đầu ra kỹ thuật:** (1) source sửa tại starter/SyncEngine.cs; (2) evidence trước/sau; (3) ADR 300–500 từ trong workspace/my-investigation.md: chọn conflict policy, mô tả dữ liệu cần persist, transaction boundary, API response và trade-offs.

**Ràng buộc triển khai thật:** 10.000 Work Orders/ngày, tối đa hai thiết bị offline mỗi Work Order, p95 sync dưới 200ms trong điều kiện khỏe mạnh, không được trộn dữ liệu tenant, có audit trail 90 ngày.

**Change request sau khi verify:** Bổ sung field Status có state machine Assigned → InProgress → Completed; một thiết bị gửi Cancelled sau khi Completed. Quyết định liệu field-level merge thuần túy đã đủ chưa và cần validation ở đâu.

Lưu ý: fixture dùng in-memory để làm rõ cơ chế. Đây không phải thiết kế persistence production hoàn chỉnh.
