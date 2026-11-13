# UNIT-PGVACUUM-001 — Báo cáo đỗ xe giữ snapshot khiến bảng PostgreSQL phình to

## Mục tiêu
Phân biệt triệu chứng bloat, lock contention và thiếu tài nguyên bằng chứng; đưa ra quyết định vận hành có thể kiểm chứng, không chữa cháy bằng tăng máy.

## Bối cảnh thực tế
Hệ thống thanh toán bãi đỗ xe ghi nhận 2 triệu lượt cập nhật trạng thái mỗi ngày. Trong 48 giờ, p95 truy vấn đối soát tăng từ 75 ms lên 780 ms. CPU còn dư; autovacuum vẫn có log hoạt động. Một tác vụ xuất báo cáo chạy vào ban đêm.

## Bạn cần làm gì
Đọc [incident evidence](evidence/incident.md), tính tốc độ tăng dead tuples, lập ít nhất ba giả thuyết và quyết định can thiệp an toàn theo các giới hạn trong [scenario](docs/scenario.md). Viết ADR vào [workspace](workspace/my-investigation.md). Đối chiếu [reference solution](solution/README.md) chỉ sau khi tự quyết định.

## Yêu cầu môi trường
Chỉ cần Markdown và máy tính; không cần PostgreSQL hay Azure. Đây là tabletop investigation với dữ liệu synthetic, không phải benchmark thực tế.

## Chạy nhanh
1. Mở `evidence/incident.md` và dựng timeline.
2. Hoàn thành bảng tính và kế hoạch kiểm chứng trong `workspace/my-investigation.md`.
3. Đề xuất hành động khẩn cấp, sửa bền vững, rollback và tiêu chí thành công.

## Cách reproduce vấn đề
Reproduce bằng cách đối chiếu các snapshot tại 00:00, 06:00, 12:00, 18:00 trong evidence; tính delta và kiểm tra giả thuyết nào giải thích đồng thời transaction age, dead tuples và p95.

## Những gì cần quan sát
Đánh giá xu hướng, phân biệt correlation với causation; chỉ rõ phép đo nào sẽ bác bỏ giả thuyết ưu tiên.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trên giấy và đánh giá rủi ro.
4. Verify bằng tiêu chí định lượng.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Một phương án có thể bảo vệ được](solution/README.md).

## Expected Results
Trước: báo cáo vẫn hoàn tất nhưng p95 và dead tuples tăng. Sau: snapshot dài được loại bỏ có kiểm soát, old snapshots được giải phóng, vacuum có cơ hội reclaim và latency được theo dõi về baseline. Không cam kết latency giảm tức thời hoặc VACUUM trả lại dung lượng OS.

## Estimated Time
65 phút.
