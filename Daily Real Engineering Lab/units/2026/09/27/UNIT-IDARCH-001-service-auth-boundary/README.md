# UNIT-IDARCH-001 — Service-to-service authentication boundary

## Mục tiêu
Đưa ra quyết định authentication cho một internal .NET worker gọi API nội bộ trên Azure, dựa trên security boundary, vận hành và blast radius.

## Bối cảnh thực tế
Một BackgroundService chạy trên Azure App Service cần gọi Inventory API. Hệ thống hiện truyền một shared API key qua configuration. Security review yêu cầu giảm long-lived credentials, hỗ trợ rotation/audit tốt hơn và không làm developer workflow quá phức tạp.

## Bạn cần làm gì
Đọc scenario.md, viết decision trước, sau đó so sánh reference solution. Nêu trust boundary, credential lifecycle, failure mode, local-development story và migration path.

## Yêu cầu môi trường
Không cần cloud subscription. Đây là Design Decision Lab.

## Chạy nhanh
1. Đọc scenario.md.
2. Viết ADR ngắn: option, assumptions, trade-offs, rollout, rollback.
3. Stress-test với requirement change.
4. Xem solution/README.md.

## Cách reproduce vấn đề
Không áp dụng: lab đánh giá design boundary, không mô phỏng runtime bug.

## Những gì cần quan sát
Identity issuer, API trust, secret lifecycle, blast radius, auditability, local development và CI/CD identity.

## Quy tắc làm lab
1. Ghi assumptions trước.
2. So sánh ít nhất 3 options.
3. Chọn theo constraints.
4. Mô tả rollout/rollback.
5. Sau đó mới xem solution.

## Hints
Dùng constraint matrix trong scenario.

## Reference Solution
[Reference Solution — spoiler](solution/README.md)

## Expected Results
Một ADR có decision, rejected alternatives, security/operations trade-offs và migration plan kiểm chứng được.

## Estimated Time
45–60 phút.
