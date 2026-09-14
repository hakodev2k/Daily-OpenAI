# Scenario

Hệ thống commerce có ASP.NET Core API chạy nhiều instance. Mỗi đêm cần reconciliation khoảng 200.000 records với partner API.

## Hiện trạng

- Job chạy bằng `BackgroundService` bên trong web application.
- Một lần chạy bình thường mất 35–55 phút.
- Deploy có thể diễn ra bất kỳ lúc nào trong ngày; emergency deploy ban đêm không bị cấm.
- App Service có thể scale in.
- Job có checkpoint theo page nhưng retry hiện còn đơn giản.
- API và batch dùng chung CPU, memory và connection pools.

## Constraints

- Team backend: 4 developers, không có platform team riêng.
- Azure đã được sử dụng, nhưng team muốn tránh vận hành thêm service nếu lợi ích không rõ ràng.
- Chấp nhận job hoàn thành trễ tối đa 2 giờ.
- Không được xử lý mất record.
- Xử lý lặp một record có thể chấp nhận nếu downstream operation được làm idempotent.
- Chi phí cloud tăng nhẹ là chấp nhận được, nhưng không được tăng mạnh chỉ để có kiến trúc “đẹp”.
- Trong 6 tháng tới volume có thể tăng 3 lần.

## Options tối thiểu cần đánh giá

1. Giữ `BackgroundService` trong web application.
2. Tách thành .NET Worker Service riêng.
3. Azure Functions / timer-triggered or queue-driven processing.

Bạn có thể đề xuất option thứ tư nếu nó thực sự hợp lý.

## Deliverable

Ghi vào `workspace/my-decision.md`:

- assumptions
- decision criteria
- comparison table
- selected option
- failure handling model
- deployment/operations implications
- trigger để revisit quyết định
