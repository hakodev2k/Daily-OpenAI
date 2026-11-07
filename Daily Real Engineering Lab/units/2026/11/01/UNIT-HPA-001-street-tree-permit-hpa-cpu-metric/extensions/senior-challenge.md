# Senior challenge — thay đổi yêu cầu

Sau khi sửa HPA, nhiều request chờ I/O: CPU thấp nhưng p95 vẫn cao. Team đề xuất thêm memory metric, maxReplicas=30 và giảm requests để scale nhanh.

Viết ADR ngắn: CPU có còn đại diện bottleneck? Nên chọn RPS, queue depth hay custom metric? HPA xử lý nhiều metrics ra sao? Request sizing tác động scheduling/cost thế nào? Startup/readiness và downstream capacity có giới hạn gì? Không tự động thay đổi starter trong bài chính.
