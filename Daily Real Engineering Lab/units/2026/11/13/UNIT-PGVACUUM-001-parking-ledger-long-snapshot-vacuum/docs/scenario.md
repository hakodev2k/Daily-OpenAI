# Constraints & decision brief

- PostgreSQL 16, một primary, 2 read replicas; dữ liệu và metrics đều synthetic.
- Ledger có 25 triệu row; UPDATE thường xuyên, retention 18 tháng.
- Báo cáo cuối ngày phải nhất quán theo thời điểm chốt, hoàn tất trong 45 phút; không được làm mất hoặc đếm đôi giao dịch.
- Tác vụ xuất hiện chạy 4 giờ, read transaction ở isolation REPEATABLE READ.
- Đội vận hành chỉ có 2 người; downtime ghi dữ liệu tối đa 2 phút; không được dùng VACUUM FULL trên primary giờ cao điểm.
- Yêu cầu bảo mật: không ghi nội dung biển số vào logs.
- Cân nhắc 3 lựa chọn: A giữ transaction snapshot xuyên suốt export; B tạo tập khóa báo cáo tại cutoff rồi đọc theo batch transaction ngắn; C chuyển report sang replica nhưng giữ snapshot dài.
- Bổ sung thay đổi yêu cầu: từ tháng sau báo cáo có thể tăng gấp 4 và phải replay theo cutoff của hôm trước.
- Đầu ra: ADR có invariant, dữ liệu cần đo, triển khai theo giai đoạn, rollback, vận hành, chi phí và regression cases.
