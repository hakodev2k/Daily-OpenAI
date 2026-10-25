# Case brief — CivicArchive

## Hệ thống hiện tại

CivicArchive cung cấp ASP.NET Core API, SQL Server làm source of truth, Elasticsearch phục vụ public search, Redis cho cache kết quả, Azure Blob Storage cho attachments và worker chạy projection/reindex. Mọi dependency trong case đều là mô phỏng trên giấy; không cần account thật.

## Quy mô và SLO

| Constraint | Giá trị |
|---|---|
| Đơn vị hành chính | 120 tenants, cách ly dữ liệu theo tenant |
| Hồ sơ hoạt động | 2.4 triệu, trung bình 2 attachments/hồ sơ |
| Search | peak 400 requests/s, tối đa 20 hits/request |
| Latency | public search p95 dưới 250 ms ở tải bình thường |
| Yêu cầu gỡ | peak 5 requests/s, có thể retry |
| Xác nhận với người dùng | trong 2 phút nếu đã an toàn về public visibility; nếu chưa thì báo PENDING |
| Sau khi xác nhận | không được trả nội dung hồ sơ qua search, detail hoặc download |
| Dữ liệu active không thuộc legal hold | purge trong 24 giờ |
| Backup | giữ 30 ngày, không được phục vụ public traffic trực tiếp |
| Legal hold | khoảng 2% hồ sơ, cần retention hợp lệ nhưng phải đánh giá quyền hiển thị riêng |
| Regions | một region chính, một region dự phòng; failover trong 60 phút |
| Team | 4 backend, 1 SRE part-time; ưu tiên giải pháp vận hành được |

Đây là ràng buộc sản phẩm giả định cho lab, **không phải diễn giải nghĩa vụ pháp lý**. Nếu retention hoặc quyền xoá có tranh chấp, yêu cầu review bởi privacy/legal.

## Câu hỏi quyết định

Đề xuất thiết kế thỏa public visibility invariant, có audit tối thiểu, không làm search quá chậm, không đòi distributed transaction trên SQL + Redis + search + Blob, và chịu được worker crash, event duplicate, stale reindex, backup restore.

So sánh tối thiểu:
- A. Xoá đồng bộ tất cả stores trước khi trả 200.
- B. Luồng bất đồng bộ với một cơ chế bảo vệ public visibility có thể kiểm chứng.
- C. Giảm số lượng bản sao hoặc chuyển search sang một nguồn khác.

Không bắt buộc chọn B. Ghi rõ vì sao phương án của bạn thỏa hoặc không thỏa từng constraint.

## Deliverables

1. Architecture Decision Record: context, options, decision, consequences, rollback triggers.
2. Data-flow/sequence diagram: delete request, read/search/download, reindex, recovery.
3. State machine và invariant cho acknowledge.
4. Failure matrix có ít nhất 6 trường hợp.
5. Kiểm thử quan sát được, metric, alert, owner, runbook.
6. Chi phí và giới hạn giải pháp, bao gồm legal hold, backup và link đã phát hành.
