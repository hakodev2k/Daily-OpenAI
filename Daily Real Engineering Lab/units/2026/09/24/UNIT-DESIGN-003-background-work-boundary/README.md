# UNIT-DESIGN-003 — Chọn boundary cho background work

## Mục tiêu
Đưa ra quyết định kỹ thuật cho một workload xử lý tài liệu bất đồng bộ với yêu cầu latency, retry và vận hành rõ ràng.

## Bối cảnh thực tế
Một API nhận tài liệu và cần thực hiện bước xử lý kéo dài 10–90 giây. Team đang cân nhắc xử lý ngay trong request, dùng BackgroundService, hoặc tách qua queue và worker.

## Bạn cần làm gì
Phân tích constraints, chọn một phương án có thể bảo vệ được, nêu failure modes, retry boundary, ownership của work item và observability cần có.

## Constraints
- API phải phản hồi trong 2 giây.
- Mỗi tài liệu phải được xử lý ít nhất một lần.
- Duplicate delivery có thể xảy ra.
- Deploy không được làm mất work đã được chấp nhận.
- Team nhỏ, ưu tiên vận hành đơn giản.

## Quy tắc làm lab
Viết decision trước khi xem reference solution. Không chọn công nghệ chỉ vì nó “senior” hơn.

## Success criteria
Giải pháp phải giải thích rõ durability, idempotency, retry, shutdown/deploy behavior và operational cost.

## Estimated Time
45 phút.