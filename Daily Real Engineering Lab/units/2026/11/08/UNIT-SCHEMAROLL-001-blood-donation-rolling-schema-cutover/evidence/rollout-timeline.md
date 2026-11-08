# Evidence packet — simulated incident

**Dữ liệu giả lập cho lab**, không phải incident thật.

## Hợp đồng hiện tại
- V1 (API + worker): đọc/ghi `donor_profiles.sms_opt_in BOOLEAN NOT NULL DEFAULT false`.
- V2 (API + worker): đọc/ghi `donor_profiles.contact_preference TEXT NOT NULL`, giá trị `SMS` hoặc `NONE`.
- Bản ghi có thể được cập nhật bởi cả API và worker nhắc lịch. Audit phải ghi nhận trạng thái consent chính xác; không được tự suy luận `NULL` thành đồng ý.
- DB PostgreSQL 15, 8 API replicas, 4 workers; rolling deployment 25 phút. Không được dừng ghi quá 30 giây.
- Dữ liệu 12 triệu donor profiles; mục tiêu backfill 2000 hàng/batch; đội vận hành 3 người; không thêm dịch vụ mới trong sprint này.

## Timeline
| Giờ | Thao tác / telemetry | Quan sát |
|---|---|---|
| 09:00 | 8 API V1, 4 worker V1 | error rate 0.1%, 0 missing consent |
| 09:03 | Migration đổi tên `sms_opt_in` thành `contact_preference` và chuyển kiểu dữ liệu | schema migration kết thúc |
| 09:05 | 2 API V2, 6 API V1; worker V1 vẫn hoạt động | lỗi 500 tăng lên 18% ở V1; V2 ổn |
| 09:09 | 5 API V2, 3 API V1 | lỗi giảm theo số V1 còn lại |
| 09:13 | 8 API V2, 4 worker V1 | 12.000 retry từ worker; số notification mismatch |
| 09:20 | V2 gặp lỗi unrelated ở search endpoint | đề nghị rollback API sang V1 |
| 09:25 | Rollback V1 trong staging | V1 không ghi được; phục hồi schema bằng migration ngược làm mất cập nhật V2 |

## Dữ liệu gây nhiễu
- CPU database 32–38% suốt incident.
- Cache hit rate giảm 2 điểm phần trăm.
- Một endpoint reporting chậm thêm 15 ms.

## Yêu cầu acceptance
- Trong rollout và rollback, V1/V2 có thể đọc/ghi dữ liệu theo hợp đồng tương ứng.
- Không mất cập nhật consent; mỗi thay đổi có audit record và cách xử lý concurrent writes rõ ràng.
- Backfill có thể resume, không khoá bảng dài, có đối soát và ngưỡng lỗi.
- Contract chỉ được phép sau khi không còn V1 API, worker, scheduled jobs và rollback window đã đóng.
- Có tín hiệu quan sát được để ra quyết định go/no-go, không dựa vào giả định deployment đã xong.
