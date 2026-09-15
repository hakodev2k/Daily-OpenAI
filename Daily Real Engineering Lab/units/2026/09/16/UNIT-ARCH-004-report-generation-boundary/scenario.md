# Scenario

## Current system
- ASP.NET Core application, 3 replicas.
- SQL Server là source chính.
- Export CSV hiện chạy trong request và stream kết quả về client.
- Team 5 developers, không có dedicated platform/SRE.
- Deployment trung bình 3 lần/tuần.

## Workload evidence
- 85% export: 5k–30k rows, 2–8 giây.
- 12% export: 30k–150k rows, 8–35 giây.
- 3% export: 150k–600k rows, 35–140 giây.
- Forecast 6 tháng: nhóm lớn có thể đạt 1.5M rows.
- Peak: 12 concurrent exports.

## Incident evidence
Trong 30 ngày gần nhất:
- 7 request bị client/proxy timeout trong khi server vẫn tiếp tục một phần công việc.
- 2 deployment restart làm mất export đang chạy.
- Một số user retry thủ công vì không biết job cũ còn chạy hay không.
- Không có yêu cầu report phải hoàn thành tức thì; business chấp nhận 2–5 phút cho export lớn.

## Constraints
- Không được tạo duplicate expensive work nếu user retry cùng một export request.
- User cần thấy trạng thái: queued/running/completed/failed.
- Report hoàn thành phải có thể download trong 24 giờ.
- Không được tăng đáng kể operational burden nếu chưa có nhu cầu rõ ràng.
- Budget cloud tăng thêm tối đa khoảng 150 USD/tháng ở giai đoạn này.
- Không có yêu cầu multi-region.

## Candidate options
### A — Giữ synchronous HTTP
Tiếp tục xử lý trong request, cải thiện timeout/streaming/query.

### B — Background processing trong cùng deployable
HTTP tạo export request; background worker trong cùng application xử lý và lưu artifact.

### C — Queue + worker deployable riêng
Web enqueue work; worker riêng consume, tạo artifact và cập nhật status.

## Deliverable
Chọn một option chính cho 6 tháng tới. Bạn có thể đề xuất biến thể, nhưng phải nêu:
- assumptions
- reliability semantics
- duplicate handling
- deployment behavior
- observability
- cancellation/retry policy
- cost/operations
- migration path
- measurable triggers để revisit decision.