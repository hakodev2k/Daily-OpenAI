# UNIT-KEYROTATE-001 — Màn hình cảnh báo đô thị mất kết nối sau bảo trì khóa

## Mục tiêu
Phân tích sự cố tích hợp Azure Key Vault và dịch vụ ký yêu cầu; xây dựng phương án rotation không gián đoạn, có thể kiểm chứng và rollback.

## Bối cảnh thực tế
Hệ thống biển báo giao thông lấy credential từ Key Vault để gọi gateway. Sau lịch rotation 02:00, một số instance trả 401 kéo dài; số khác vẫn hoạt động. Nhóm vận hành muốn bỏ rotation để tránh downtime nhưng chính sách bảo mật không cho phép.

## Bạn cần làm gì
Dựa trên [evidence](evidence/incident.md), dựng timeline, phân biệt giả thuyết và bằng chứng, viết ADR vào [workspace](workspace/decision.md), thiết kế quy trình rotation và kế hoạch xác minh. Không cần Azure subscription.

## Yêu cầu môi trường
Markdown editor; tùy chọn PowerShell 7 để chạy bộ kiểm tra tài liệu. Không cần cloud hoặc package ngoài.

## Chạy nhanh
Mở `evidence/incident.md`, điền `workspace/decision.md`, rồi chạy `./verify.ps1` để kiểm tra đủ deliverables. Kiểm tra tự động chỉ xác nhận cấu trúc; chất lượng quyết định cần đối chiếu rubric.

## Cách reproduce vấn đề
Đọc timeline 01:55–02:25 và đối chiếu version ID, tuổi cache, trạng thái instance và mã lỗi trong evidence. Dựng ít nhất hai cách giải thích có thể bị bác bỏ.

## Những gì cần quan sát
Sự phân bố lỗi theo instance, độ trễ khi cấu hình thay đổi, ranh giới giữa secret storage, token validation và quá trình rollout.

## Quy tắc làm lab
1. Phân tích evidence trước.
2. Ghi hypothesis và phép kiểm chứng.
3. Thiết kế phương án kèm trade-offs.
4. Verify và review.
5. Chỉ sau đó mở solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md)

## Expected Results
ADR nêu được lựa chọn, giả định, failure paths, rollout, rollback, security controls, SLO và test matrix. Không chấp nhận giải pháp chỉ đơn giản tắt rotation.

## Estimated Time
45–55 phút.
