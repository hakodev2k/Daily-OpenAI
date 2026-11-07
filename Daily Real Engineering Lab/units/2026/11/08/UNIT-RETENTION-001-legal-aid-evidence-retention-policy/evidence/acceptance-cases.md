# Acceptance cases — tự đánh giá trước khi xem lời giải

Với mỗi case, điền: expected decision, persisted state, audit reason, external operation, retry/reconcile behavior.

1. **Open case, old upload:** file 400 ngày tuổi, case chưa đóng. Không được mất file.
2. **Closed case, hold:** đóng 500 ngày, hold đang hiệu lực, yêu cầu xóa đến. Phải bảo toàn file và ghi nhận request.
3. **Closed case, retention chưa hết:** đóng 300 ngày, có yêu cầu xóa. Trạng thái chờ và deadline phải xác định được.
4. **Eligible deletion:** đóng 400 ngày, không hold; worker crash sau khi storage xác nhận delete nhưng trước khi DB commit. Retry phải hội tụ đúng, không ghi audit thành công sai.
5. **Hold race:** worker đọc trạng thái eligible, người dùng đặt hold trước external delete. Chứng minh không có cửa sổ xóa trái chính sách bằng serialization/fencing hoặc storage-enforced hold.
6. **New upload after case hold:** hold được đặt lúc 10:00, object upload lúc 10:01. Object mới phải được bảo vệ ngay, kể cả khi event propagation trễ.

Chỉ đạt khi mô tả được hành vi trong crash, concurrent update, delayed event và failed storage API; đừng chỉ mô tả happy path.
