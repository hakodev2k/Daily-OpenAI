# UNIT-SCHEMAMIG-001 — Triển khai NOT NULL không gián đoạn cho dữ liệu trạm xe đạp

## Mục tiêu
Thiết kế và kiểm chứng kế hoạch thay đổi schema PostgreSQL khi nhiều phiên bản ASP.NET Core API và worker cùng ghi dữ liệu. Phân biệt migration thành công với hệ thống thực sự tương thích.

## Bối cảnh thực tế
Nền tảng xe đạp công cộng lưu sự kiện dock vào `dock_events`. Báo cáo vận hành mới yêu cầu mỗi sự kiện có `station_zone`. API, worker nhập sự kiện offline và dashboard được triển khai theo từng nhóm pod; không thể dừng tiếp nhận sự kiện.

## Bạn cần làm gì
1. Đọc [bằng chứng và timeline](evidence/rollout-timeline.md), xác định ít nhất ba giả thuyết.
2. Chọn chiến lược migration, trình bày các bước deploy, backfill, xác minh và rollback trong [workspace](workspace/my-investigation.md).
3. Đánh giá từng lựa chọn qua [bảng tiêu chí](docs/acceptance-matrix.md); phân biệt khả năng rollback ứng dụng với rollback dữ liệu.
4. Sau khi hoàn thành, đối chiếu [reference solution](solution/README.md).

## Yêu cầu môi trường
Markdown editor và kiến thức cơ bản về PostgreSQL, SQL, ASP.NET Core. Không cần Azure, database thật hay gói trả phí. Đây là design-decision tabletop, không có executable starter hoặc lệnh giả lập.

## Chạy nhanh
Mở `evidence/rollout-timeline.md` → `docs/acceptance-matrix.md` → ghi quyết định vào `workspace/my-investigation.md`. Có thể dùng VS Code để xem Markdown.

## Cách reproduce vấn đề
Tái dựng các bước T0–T5 trong timeline; đối chiếu DDL dự kiến với hai loại writer và các row cũ. Ghi rõ bước nào gây ra lỗi và bằng chứng cần thu thập khi chạy staging.

## Những gì cần quan sát
Tỷ lệ insert lỗi theo phiên bản, số row chưa đủ dữ liệu, độ trễ replication, mức độ lock, trạng thái backfill, khả năng ghi của worker offline.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix hoặc phương án thiết kế.
4. Verify bằng acceptance matrix và rollback drill.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Phương án tham khảo và trade-offs](solution/README.md) — chỉ mở sau khi tự quyết định.

## Expected Results
Trước: một số phiên bản ghi lỗi khi thay đổi schema, số lượng bản ghi thiếu zone còn tồn tại. Sau: tất cả writer tương thích, dữ liệu cũ được xử lý, constraint được áp dụng sau khi kiểm chứng và có kế hoạch rollback rõ ràng.

## Estimated Time
50 phút (D4 / L5).