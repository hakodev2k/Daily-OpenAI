# Reference Solution

> Chỉ xem sau khi bạn đã tự đưa ra quyết định.

## Kết luận

Một phương án defensible cho constraints hiện tại là **tách reconciliation thành một .NET Worker Service riêng**, giữ cơ chế checkpoint/idempotency rõ ràng và triển khai độc lập với web API.

Đây không phải lựa chọn duy nhất đúng. Azure Functions cũng hợp lý nếu team đã quen với Functions và muốn scale/trigger managed hơn. Giữ `BackgroundService` trong web app chỉ phù hợp nếu chấp nhận coupling với lifecycle của web host và workload đủ nhỏ.

## Symptoms / Evidence

- Job hiện phụ thuộc lifecycle của ASP.NET Core web host.
- Deploy hoặc scale-in có thể ngắt batch giữa chừng.
- API và batch chia sẻ CPU, memory và connection pools.
- Volume dự kiến tăng 3 lần trong 6 tháng.

## Root Cause

Vấn đề kiến trúc chính không phải `BackgroundService` tự thân bị lỗi. Vấn đề là workload dài hạn, có yêu cầu completion/recovery riêng, đang bị **couple lifecycle và resource boundary** với web application.

## Vì sao Worker Service phù hợp

- Tách deployment lifecycle khỏi API.
- Tách resource envelope và scaling policy.
- Vẫn giữ mô hình .NET quen thuộc cho team 4 người.
- Không bắt buộc thay đổi ngay sang event-driven architecture.
- Dễ thêm checkpoint, retry và idempotency theo batch boundary.

## So sánh alternatives

### BackgroundService trong web app

Ưu điểm: ít infrastructure, dễ triển khai.

Nhược điểm: deploy/recycle/scale-in của web ảnh hưởng trực tiếp; batch cạnh tranh tài nguyên với request traffic.

### Dedicated Worker Service

Ưu điểm: isolation tốt hơn, operational model vẫn đơn giản, deploy độc lập, dễ scale theo batch.

Nhược điểm: thêm một deployable unit và monitoring surface.

### Azure Functions

Ưu điểm: managed trigger, có thể scale tốt, phù hợp khi chia nhỏ work thành các item độc lập.

Nhược điểm: cần thiết kế execution/retry/idempotency kỹ; workload 35–55 phút không nên chỉ chuyển nguyên khối sang một invocation rồi coi là xong.

## Failure Handling

- Chia work thành page/chunk có checkpoint.
- Mỗi chunk phải retry an toàn.
- Downstream mutation cần idempotency key hoặc semantic deduplication.
- Sau restart, worker resume từ checkpoint đã durable thay vì bắt đầu lại mù quáng.
- Theo dõi progress, retry count, failed chunk và total completion time.

## Wrong / Tempting Decisions

- Chọn Azure Functions chỉ vì “serverless = scalable”.
- Giữ job trong API chỉ vì hiện tại code đã nằm đó, bỏ qua failure boundary.
- Tạo microservice + queue + orchestrator ngay lập tức dù constraints chưa cần.
- Chỉ tăng instance web để giảm resource contention mà không xử lý lifecycle coupling.

## Production Implications

Cần dashboard riêng cho batch completion, retry, checkpoint age và duration. Deployment pipeline nên cho phép deploy worker độc lập. Nếu job cần chạy đúng một active instance, ownership/leader semantics phải được định nghĩa rõ.

## Trade-offs

Worker Service tăng một chút operational overhead để đổi lấy isolation và control. Với team nhỏ, đây thường là mức complexity hợp lý hơn so với chuyển ngay sang orchestration phức tạp.

## Khi nào nên revisit

- Volume tăng vượt khả năng một worker xử lý trong SLA.
- Cần near-real-time thay vì nightly batch.
- Partner API rate limits yêu cầu fan-out có kiểm soát.
- Nhiều domain cùng cần workflow orchestration.
- Team đã có platform primitives tốt cho Functions/queues.

## Senior Insight

Quyết định không nên bắt đầu bằng “chọn công nghệ nào”, mà bằng **failure boundary, lifecycle ownership, resource isolation, recovery semantics và operational cost**. Công nghệ chỉ là cách hiện thực hóa các boundary đó.
