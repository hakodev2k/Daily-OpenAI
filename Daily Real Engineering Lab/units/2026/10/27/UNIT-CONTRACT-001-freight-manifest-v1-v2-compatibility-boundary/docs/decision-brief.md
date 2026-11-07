# Decision brief — live version migration

## Constraints
2.000 manifests/phút; p95 ingest mục tiêu < 300 ms; 35% v1, 65% v2. Không silent data corruption, không ngắt toàn bộ partner. Rollback dưới 15 phút; sunset v1 sau thông báo tối thiểu 60 ngày. Log lý do reject nhưng không ghi payload nhạy cảm.

## Hands-on architecture task
So sánh: A) một DTO v2 duy nhất; B) version-specific ingress adapters + canonical model; C) hai pipeline end-to-end. Viết ADR vào `workspace/my-investigation.md`: decision, rejected options, operational cost, rollout, canary, rollback triggers, observability và migration plan.

## Requirement change
Ngày thứ 20, Partner C gửi `schemaVersion: 3` chưa được hỗ trợ. Quy định cách trả lỗi rõ ràng mà không âm thầm diễn giải v3 thành v2. Mô tả cách nâng cấp contract tests khi v3 chính thức ra mắt.
