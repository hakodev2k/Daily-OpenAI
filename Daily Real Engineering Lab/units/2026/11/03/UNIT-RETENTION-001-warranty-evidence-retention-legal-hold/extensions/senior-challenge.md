# Senior Challenge — requirement change
Business yêu cầu xóa theo yêu cầu người dùng ngay (trừ hold), storage region A outage 36 giờ, và DR restore PostgreSQL backup 14 ngày tuổi. Sửa ADR để xác định precedence, khả năng đáp ứng SLA, replay tombstones/hold changes trước khi mở read traffic, và điều gì không thể bảo đảm khi physical delete đã xảy ra trước hold acceptance.
