# Yêu cầu sản phẩm và giới hạn

ASP.NET Core API v1 ghi trực tiếp PostgreSQL 15: `pickup_route(id bigint PK, household_id uuid, service_zone text NOT NULL, route_status text, updated_at timestamptz)`. API v2 sẽ dùng `zone_id uuid` tham chiếu bảng `zone`. Trong rolling deploy tối đa 20 phút, cả hai phiên bản đều có thể ghi. V1 không biết `zone_id`; v2 có thể ghi `zone_id` nhưng vẫn phải hỗ trợ rollback.

- 12.000.000 tuyến: 11.840.000 mapping duy nhất, 110.000 không tìm thấy, 50.000 mơ hồ.
- Peak 120 updates/s; p95 write ≤ 250 ms; DB CPU baseline 65%.
- Availability ≥ 99,95%; không planned downtime; rollback ứng dụng trong 30 phút.
- Tuyến chưa xác minh không được tự động dispatch sai. Backfill phải throttle, pause/resume và replay an toàn.
- 14 ngày cho migration, manual review có thể lâu hơn. Đội 4 backend, 1 DBA part-time, 2 operations; không có ngân sách streaming platform mới.

**Deliverables:** so sánh ≥2 phương án; deployment/rollback phases; cách xử lý v1/v2 đồng thời, ambiguous/unmapped, backfill crash và concurrent writes; ≥5 cutover metrics có ngưỡng; người có quyền dừng và phê duyệt irreversible contract.

Đây là mô phỏng; mọi số liệu đều synthetic. Không yêu cầu cloud, Docker hoặc database.
