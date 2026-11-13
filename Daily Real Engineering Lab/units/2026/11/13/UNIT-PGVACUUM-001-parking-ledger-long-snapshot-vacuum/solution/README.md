# Reference Solution — chỉ xem sau khi đã tự điều tra

## Symptoms và evidence
Dead tuples tăng 2,420,000 trong 18 giờ, trung bình khoảng 134,444/giờ. p95 tăng từ 75 lên 780 ms (10.4x) dù CPU chỉ tăng 39% → 47%. Autovacuum counter tăng 14 lần, vì thế câu trả lời “bật autovacuum” không đủ. Transaction age tăng đều từ phút lên 18 giờ; không có lock waits kéo dài.

## Root cause / competing hypotheses
Giả thuyết mạnh nhất: export worker giữ REPEATABLE READ transaction lâu, giữ xmin horizon khiến VACUUM không thể thu hồi một số row versions. Cần xác nhận backend_xmin của session, tuổi snapshot và quan sát reclaim sau khi transaction kết thúc. Các giả thuyết khác: query-plan regression sau deploy, index bloat độc lập, thiếu index cho analytics, replication slot giữ xmin; phải đo chứ không loại bỏ chỉ bằng timeline. n_dead_tup là estimate và không chứng minh riêng nguyên nhân.

## Decision
Chọn B, nhưng **chỉ nếu** có thể xây tập row identity + version hoặc immutable report snapshot tại cutoff. Việc lấy danh sách ID rồi đọc giá trị mutable theo batch không đảm bảo số liệu nhất quán. Có thể lưu immutable settlement events với sequence watermark hoặc materialize versioned report facts vào bảng staging trong transaction ngắn, sau đó export từng trang theo keyset. Ghi cutoff, schema version, manifest checksum và export progress để replay an toàn. Không giữ transaction hàng giờ.

## Containment
1. Xác minh session owner và snapshot; phối hợp dừng/cancel export có kiểm soát, không kill mù; bảo toàn checkpoint.
2. Giới hạn concurrency của export, timeouts, alert transaction age và autovacuum effectiveness; theo dõi p95, dead tuple estimates, disk và WAL.
3. Vacuum thông thường sau khi xmin horizon giải phóng; không chạy VACUUM FULL trên primary giờ cao điểm. Không kỳ vọng disk OS thu hồi ngay.

## Verify
- Không có report transaction > 5 phút (định nghĩa alert, có ngoại lệ được duyệt).
- 24 giờ sau: n_dead_tup có xu hướng giảm so với baseline churn, vacuum có reclaim thực tế, p95 cải thiện trong cùng traffic mix; ghi số đo và confidence.
- Với 10,000 row synthetic và 100 concurrent updates: report tại cùng cutoff có cùng checksum, không mất/đếm đôi; replay và crash-restart idempotent.
- Canaries: export một tenant trước, compare totals/checksum, rollback worker nếu invariant sai.

## Alternatives / trade-offs
A đơn giản, nhất quán nhưng snapshot kéo dài ảnh hưởng vacuum và khả năng mở rộng. C replica giảm query load primary nhưng replica hot standby có thể conflict với vacuum và feedback có thể ảnh hưởng primary; không tự giải quyết transaction design. B thêm storage và complexity nhưng giới hạn transaction lifetime, replay được. Cân nhắc materialized snapshot chỉ khi cost chấp nhận được.

## Wrong fixes
Tăng CPU/replicas không giải phóng xmin horizon; tăng autovacuum workers không vượt qua snapshot còn sống; VACUUM FULL gây exclusive lock; chỉ chuyển query sang replica có thể đổi triệu chứng mà không chữa gốc.

## Production implications / Senior lens
Tách consistency boundary của báo cáo khỏi lifetime của kết nối/transaction; có ownership, SLO, rollback, cutoff reproducibility, dữ liệu riêng tư và kiểm chứng theo cohort. Với yêu cầu 4x, shard export theo stable partition và manifest versioned; không chạy nhiều transaction dài song song.
