# Expected results — trước khi sửa release plan

Trong timeline giả lập:
- V1.0 vẫn truy vấn cột legacy trong khi schema đã đổi; cohort V1.0 có lỗi SQL 207 và HTTP 5xx.
- V2 đọc schema mới bình thường, nên tổng error rate che khuất sự khác biệt theo version.
- Rollback ứng dụng về V1.0 không sửa được schema; lỗi vẫn tồn tại.
- CPU và connection count ổn định không loại trừ contract incompatibility.
- Một kế hoạch chỉ thêm cột và backfill một lần nhưng không quản lý concurrent writes vẫn có nguy cơ stale reads.

Nếu không tái hiện được: xem lại ma trận S0/S1/S2, phân biệt rename vật lý với alias API, và kiểm tra trường hợp writer cũ cập nhật sau backfill.
