# Reference Solution — inspect only after completing your own decision

## Một phương án defensible

Với constraints hiện tại, chọn **Dedicated .NET Worker + durable queue** là phương án cân bằng tốt nếu media processing thực sự cần tồn tại độc lập với request lifecycle. Web app chỉ enqueue work; worker consume và xử lý.

Đây không phải đáp án duy nhất. `BackgroundService` trong web app có thể phù hợp nếu work có thể mất/rebuild dễ dàng và team chấp nhận coupling với App Service lifecycle. Azure Functions cũng hợp lý nếu đội đã có operational maturity với Functions và muốn event-driven scaling.

## Vì sao không mặc định dùng BackgroundService trong web app

Work kéo dài tới 30 giây và phải retry đáng tin cậy. Nếu chỉ giữ work trong process memory, recycle/deploy/scale-in có thể làm mất work. Một durable queue mới là boundary quan trọng; việc host consumer ở đâu là quyết định thứ hai.

## Vì sao không mặc định dùng Azure Functions

Peak 20 jobs/phút còn nhỏ. Auto-scale không phải constraint quyết định. Thêm hosting model mới chỉ vì "serverless" có thể tăng deployment, debugging và observability complexity mà chưa tạo business value tương ứng.

## Reliability model

- Request upload persist media metadata trước.
- Sau commit thành công, enqueue message có stable job identifier.
- Consumer idempotent theo job/media version.
- Transient failures retry với giới hạn rõ ràng.
- Work vượt retry budget được đưa vào poison/dead-letter path để điều tra.
- Metrics tối thiểu: queue depth, age of oldest message, success/failure count, processing duration, retry count.

## Trade-offs

Dedicated Worker tăng thêm một deployable unit và cần queue infrastructure. Đổi lại, lifecycle của background processing không còn phụ thuộc trực tiếp vào web request host và có thể scale/deploy độc lập.

## Tempting fixes / decisions

### "Cứ dùng `Task.Run` sau upload"

Không tạo durability boundary và vẫn phụ thuộc process lifetime.

### "Dùng Azure Functions vì tự scale"

Scale không phải bottleneck hiện tại; quyết định này bỏ qua team capability và operational cost.

### "Dùng microservice riêng ngay"

Một worker riêng không đồng nghĩa cần một microservice architecture đầy đủ. Có thể giữ cùng codebase/module boundaries và chỉ tách process host.

## Khi nào nên re-evaluate

1. Peak tăng đủ lớn để worker capacity hoặc queue latency không còn đạt SLO.
2. Media processing cần scale profile hoặc security boundary khác hẳn CMS.
3. Team chuẩn hóa Azure Functions và operational overhead của Functions thấp hơn dedicated worker.

## Senior engineer nên nhận ra

Câu hỏi quan trọng nhất không phải "Worker hay Functions" mà là: work có cần durable handoff khỏi request lifecycle không, delivery semantics là gì, idempotency nằm ở đâu, và team có thể vận hành giải pháp đó đáng tin cậy hay không.
