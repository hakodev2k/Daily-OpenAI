# Scenario và constraints

Sở dịch vụ công xử lý 1,8 triệu hồ sơ/năm. Mỗi hồ sơ phát sinh trung bình 12 audit events; payload event sau nén 1,2 KiB, metadata và index overhead ước tính thêm 45% (chỉ dùng để lập mô hình, không coi là benchmark). Hồ sơ nghiệp vụ phải truy vấn trong p95 < 250 ms cho 95% requests, audit retrieval theo case ID p95 < 5 giây với dữ liệu trong 7 năm.

Hiện có PostgreSQL primary + một replica, một team 4 backend/DB engineers, ngân sách vận hành tăng tối đa 25%, không có dedicated data platform team. Dữ liệu audit có định danh cá nhân; auditor phải xem theo phân quyền và mọi lần đọc audit phải có access log. Phải lưu 7 năm theo chính sách nội bộ, nhưng có yêu cầu xóa/ẩn dữ liệu cá nhân khi có căn cứ pháp lý; không được giả định mọi yêu cầu xóa luôn thắng nghĩa vụ lưu trữ.

Mục tiêu: so sánh (A) cùng PostgreSQL với partition theo thời gian, (B) tách audit sang storage riêng có chỉ mục truy vấn, (C) giải pháp hybrid. Quyết định phải nêu consistency boundary, immutable/tamper-evidence, key management, restore drill, retention/erasure, chi phí, migration và rollback.

**Change request sau khi hoàn thành ADR v1:** auditor yêu cầu truy xuất toàn bộ event của một case trong 30 giây ngay cả khi một vùng cloud gặp sự cố; ngân sách vẫn giữ nguyên. Hãy nêu điều kiện cần đo/đàm phán trước khi hứa SLA.

Deliverables: ADR 1–2 trang; bảng đánh đổi; ước tính dung lượng; 3 thí nghiệm có tiêu chí pass/fail; migration và rollback.