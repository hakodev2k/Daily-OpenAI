# UNIT-AZURE-006 — Chọn Runtime Boundary cho Reconciliation Job

## Mục tiêu

Đưa ra một engineering decision có thể bảo vệ được cho workload reconciliation định kỳ giữa SQL và third-party payment provider, thay vì chọn Azure service theo thói quen.

## Bối cảnh thực tế

Một .NET backend đang chạy trên Azure App Service. Team cần reconciliation khoảng 250.000 payment records mỗi đêm. Job có thể kéo dài 25–50 phút, third-party API giới hạn 40 requests/second và đôi khi trả 429. Business chấp nhận hoàn tất trước 05:00, nhưng không chấp nhận bỏ sót record. Team có 5 backend developers và không có dedicated platform team.

Các lựa chọn đang tranh luận:

- `BackgroundService` trong web App Service hiện tại
- một worker App Service riêng
- Azure Functions timer-triggered workload

## Bạn cần làm gì

1. Xác định các failure boundaries quan trọng: process restart, deployment, scale-out, retry, duplicate execution và partial progress.
2. Chọn một phương án mặc định cho 6 tháng tới.
3. Mô tả cách checkpoint/progress, concurrency control, idempotency và observability.
4. Nêu rõ vì sao hai phương án còn lại chưa phải lựa chọn mặc định dưới constraints hiện tại.
5. Định nghĩa ít nhất 3 revisit triggers khiến decision cần được xem lại.

## Constraints

- 250k records/night, có thể tăng 3x trong 12 tháng.
- Deadline 05:00; không cần sub-second latency.
- Third-party limit 40 req/s.
- At-least-once execution là chấp nhận được nếu side effects an toàn.
- Deployment web có thể xảy ra bất kỳ ngày nào.
- Không thêm Kubernetes chỉ cho workload này.
- Team ưu tiên operational simplicity nhưng phải có recovery rõ ràng.

## Deliverable

Ghi decision vào `workspace/my-decision.md`, gồm assumptions, option matrix, chosen boundary, failure handling, observability và revisit triggers.

## Quy tắc làm lab

1. Không bắt đầu bằng tên Azure service.
2. Bắt đầu từ workload + failure semantics + operational constraints.
3. Chỉ đọc reference solution sau khi đã chốt decision của bạn.

## Reference Solution

[Spoiler — một phương án có thể bảo vệ được, không phải đáp án duy nhất](solution/README.md)

## Estimated Time

45–75 phút.