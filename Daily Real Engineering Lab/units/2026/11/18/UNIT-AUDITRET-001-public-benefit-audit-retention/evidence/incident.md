# Evidence pack — dữ liệu giả lập cho lab

- 2026-10: 22 triệu audit rows trong bảng chung, tăng khoảng 1,8 triệu rows/tháng; các giá trị tăng trưởng chưa được xác minh từ production.
- p95 truy vấn hồ sơ theo ID: 180 ms; p99: 930 ms. p95 audit-by-case: 2,4 giây; p99: 14 giây.
- Autovacuum lag tăng trong giờ import; CPU trung bình 42%; disk IOPS có spike.
- 7 năm retention: 1,8 triệu cases/năm × 12 events/case × 7 năm = 151,2 triệu events.
- Payload estimate: 151,2 triệu × 1,2 KiB × 1,45 ≈ 251 GiB, **chưa tính** replicas, WAL, backup, bloat, metadata ngoài estimate và indexing thực tế.
- Một yêu cầu khôi phục gần đây mất 3 giờ 20 phút trên staging dataset 1/4 production; chưa có bằng chứng tuyến tính để dự đoán full restore.
- Có 2 incident thiếu audit event khi nghiệp vụ commit nhưng audit publish bất đồng bộ thất bại; chưa xác định retry/transaction boundary.
- Security: 4 nhóm người có quyền đọc audit, nhưng chưa có per-case access-log coverage report.
- Chi phí hiện tại 100 đơn vị/tháng; trần mới 125 đơn vị/tháng. Chưa có báo giá object storage, egress, search index, DR và nhân công.

**Nhiễu có chủ ý:** một dashboard báo CPU dưới 50% không đủ kết luận DB không nghẽn. Không được suy ra dung lượng disk chính xác từ payload estimate.