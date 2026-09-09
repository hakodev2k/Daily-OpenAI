# Scenario

## Hiện trạng

CMS chạy ASP.NET Core trên Azure App Service. Khi editor upload media, hệ thống cần tạo một background job để đọc metadata và phát sinh thumbnail request. Hiện tại chưa có worker riêng.

## Constraints

- Khoảng 2.000 media upload/ngày, peak khoảng 20 upload/phút.
- Mỗi job thường mất 1–8 giây; một số file lớn có thể mất 30 giây.
- Không được làm chậm request upload chính.
- Job phải retry được khi lỗi transient.
- Duplicate execution có thể xảy ra và handler phải idempotent.
- Delay vài chục giây chấp nhận được.
- Không yêu cầu multi-region.
- Team backend có 3 người và ưu tiên vận hành đơn giản.
- Release 1 cần ship trong 4 tuần.
- Azure subscription đã có App Service; thêm service mới phải có lý do rõ ràng.

## Các phương án bắt buộc đánh giá

### A. `BackgroundService` trong chính web app

Đánh giá lifecycle coupling, deployment, scale-out, recycle và khả năng mất work.

### B. Dedicated .NET Worker

Đánh giá isolation, deployment overhead, queue requirement và operational ownership.

### C. Azure Functions

Đánh giá event-driven fit, scale, timeout/runtime constraints, observability và cost/complexity.

## Deliverable

Viết decision record gồm:

1. Assumptions.
2. Decision criteria có thứ tự ưu tiên.
3. Bảng trade-off ba phương án.
4. Phương án chọn cho Release 1.
5. Reliability model: enqueue, retry, idempotency, poison work.
6. Ba điều kiện cụ thể khiến kiến trúc cần được re-evaluate.
