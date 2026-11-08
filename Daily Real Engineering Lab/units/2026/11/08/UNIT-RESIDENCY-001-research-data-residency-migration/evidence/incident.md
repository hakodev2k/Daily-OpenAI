# Incident packet — Atlas tenant

Các dữ liệu dưới đây là fixture giả lập để phân tích, không phải telemetry thực.

## Hệ thống
- API Gateway → .NET API → PostgreSQL và object storage; sau commit API phát queue event.
- 4 workers xử lý file scan, PDF report và search indexing; mỗi worker cache tenant-region map trong 15 phút.
- Database EU-West đang có 1.2 triệu records, 18,000 objects; 27 queue messages chưa ACK lúc 08:59.
- Router dùng config `tenant.homeRegion`; queue messages cũ chỉ mang `tenantId`, `objectId`, `eventId`.
- Khách hàng yêu cầu **không có bản ghi cá nhân mới ở EU-West từ 09:00 UTC**, đồng thời RPO=0 với các write đã ACK; downtime tối đa 5 phút.
- Copy dữ liệu cũ được phép trong cửa sổ chuyển đổi có kiểm soát; không xóa EU-West ngay.

## Timeline giả lập
| UTC | Evidence | Ghi chú |
|---|---|---|
| 08:52 | Snapshot DB sang EU-Central hoàn thành | watermark 08:51:30 |
| 08:58 | API ghi sample S-51 ở EU-West, event E-51 pending | HTTP 201 |
| 08:59 | E-49 được worker W3 lấy, chưa ACK | worker cache EU-West |
| 09:00 | Router đổi tenant.homeRegion sang EU-Central | không restart workers |
| 09:01 | API mới ghi S-52 ở EU-Central | HTTP 201 |
| 09:02 | W3 ghi PDF của E-49 vào EU-West | ACK success |
| 09:03 | W1 nhận E-51, ghi report vào EU-West | cache cũ |
| 09:04 | EU-Central thiếu S-51 | snapshot trước commit |

## Signals
- API 5xx = 0, p95 latency ổn định.
- Queue lag tăng từ 27 lên 36 trong 5 phút.
- Storage inventory EU-West xuất hiện 2 objects mới sau 09:00.
- Một worker restart tại 09:02; 3 worker còn sống từ trước cutover.
- Không có bằng chứng DB replication bắt kịp watermark 08:59.

## Ràng buộc
Không được coi việc HTTP 200/201, queue ACK hay router config thành công là bằng chứng residency đúng. Thiết kế phải xử lý late events, retries, duplicates, crash giữa side effect và ACK, và rollback có kiểm soát.