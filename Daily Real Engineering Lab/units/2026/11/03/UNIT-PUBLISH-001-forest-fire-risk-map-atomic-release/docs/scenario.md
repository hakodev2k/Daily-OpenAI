# Scenario

Đội GIS nhận bản dự báo cháy rừng mới gồm north và south. Bản đồ phải được người trực ca đọc như một tập dữ liệu nhất quán; phiên bản cũ được giữ lại để rollback. Nhà cung cấp object storage không hỗ trợ transaction đa-object. Một lỗi giữa hai lần upload không được làm lộ bản phát hành nửa chừng.

Ràng buộc: reader không chờ upload hoàn tất, không downtime, có thể có CDN, có hai publisher chạy đồng thời. Xác định invariant trước khi thiết kế.
