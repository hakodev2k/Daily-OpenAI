# Acceptance tests — review kiến trúc và kiểm chứng

Đối với mỗi case, ghi: **input / hành động / detector / bằng chứng / kết quả / thời hạn / residual risk**. Không có mã chạy tự động; đây là bảng nghiệm thu thiết kế với fixture và fault injection giả định.

| ID | Failure injection / thay đổi | Điều cần chứng minh |
|---|---|---|
| A01 | T1 thiếu một event của B-4721 | Phát hiện lệch so với lịch sử đã chốt, không dựa chỉ vào DB hiện tại |
| A02 | Thay Actor/Action nhưng giữ nguyên số hàng | Phát hiện thay đổi nội dung, không chỉ count |
| A03 | Đổi thứ tự hai sự kiện cùng batch | Phát hiện reorder nếu order thuộc contract |
| A04 | Duplicate delivery khi retry outbox | Không tạo hai audit records có cùng business identity |
| A05 | Worker chết sau DB commit, trước checkpoint | Truy hồi backlog và báo trạng thái chưa được anchored |
| A06 | DB admin sửa cả audit table và export history | Independent verifier vẫn phát hiện khi checkpoint còn tin cậy |
| A07 | Kênh checkpoint mất 25 phút | Không báo "fully verified"; nêu SLA và cơ chế cảnh báo/giới hạn phát hành |
| A08 | Rotate signing key, verify dữ liệu ký trước rotation | Old checkpoints vẫn verify với key provenance, không tin vào key hiện tại đơn lẻ |
| A09 | Restore backup cũ rồi replay | Không tái sử dụng sequence/checkpoint sai, có reconciliation |
| A10 | Signing identity bị chiếm | Nêu rõ phạm vi không thể chứng minh, containment, revoke và incident response |
| A11 | Yêu cầu xóa dữ liệu cá nhân trong retention | Phân biệt giữ integrity proof với dữ liệu nhạy cảm; pháp lý cần xác nhận |
| A12 | Backfill legacy history chưa có anchor | Không gán nhãn "verified since origin" cho dữ liệu lịch sử |

## Quantitative gates

- Peak write: 600 events/s; p95 business write <= 300 ms.
- Integrity detection: <= 15 phút kể từ khi có evidence đã anchored, ngoại trừ outage phải được báo riêng.
- Daily independent verification: 1,2 triệu events trong <= 30 phút (~667 events/s trung bình), có kế hoạch đo throughput thực tế.
- Capacity: 0,84 GB/ngày raw; tính thêm index, bản sao, checkpoint, retention và vận hành.
- Security: verifier không dùng credential có quyền sửa audit/anchor; signer không dùng chung quyền DB admin.

**Pass** chỉ khi kiến trúc có bằng chứng thực nghiệm/đo kiểm được đề xuất cho từng gate; không coi bảng tự đánh dấu là đã test production.
