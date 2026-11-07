# Scenario và ràng buộc

Hệ thống phục vụ 35.000 hồ sơ/năm, trung bình 6 objects/hồ sơ, mỗi object 2–20 MB. Mỗi hồ sơ có CaseId; metadata trong PostgreSQL; file trong object storage; một scheduled worker chạy mỗi đêm. Mọi mốc thời gian dùng UTC. Không được coi object listing là nguồn sự thật về trạng thái hồ sơ.

**Chính sách giả định của tổ chức để làm lab (không phải tư vấn pháp lý):**
- Hồ sơ đóng: thông thường giữ file 365 ngày kể từ closedAt, rồi đủ điều kiện xóa.
- Hồ sơ đang mở: không được xóa theo tuổi object.
- Khi có hold hợp lệ: phải giữ file đến khi hold được gỡ, kể cả đã quá 365 ngày.
- Khi nhận yêu cầu xóa: ghi nhận ngay; thực thi chỉ khi không có hold và đã thỏa điều kiện retention; nếu chưa đủ điều kiện, phải thông báo trạng thái chờ và lý do.
- Mỗi lần quyết định giữ/xóa phải có audit record gồm actor, policyVersion, caseId, objectId, timestamp, decision và reason.
- Xóa là không thể đảo ngược tại application layer; bản sao dự phòng cũng cần lịch xóa phù hợp với policy và có chứng cứ.

**Hiện trạng:** tất cả objects có lifecycle expiration 365 ngày kể từ thời điểm upload, không phân biệt hồ sơ đang mở/đóng hoặc hold. Worker cập nhật cờ `deleted=true` sau khi gửi yêu cầu xóa, không kiểm tra lại object tồn tại hay xử lý kết quả bất định.

**SLO:** yêu cầu xóa đủ điều kiện được xử lý trong 48 giờ; audit phải truy vấn được trong 7 năm (lưu audit tách biệt, hạn chế PII); không để xóa object có hold còn hiệu lực. Nhân sự vận hành: 2 người, ngân sách lưu trữ giới hạn 15% tăng trưởng/năm.

**Thay đổi requirement giữa lab:** một đối tác yêu cầu hold áp dụng cho toàn bộ tài liệu của case, kể cả object mới upload sau thời điểm đặt hold. Cập nhật thiết kế để đáp ứng yêu cầu này mà không tạo khoảng trống race condition.

**Đầu ra:** ADR tối đa 2 trang, state machine, timeline xử lý hold-vs-delete, phương án backfill metadata, idempotency, failure recovery, monitoring và 6 acceptance cases. Không triển khai cloud thật.
