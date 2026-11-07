# Bằng chứng giả lập — Compliance Documents

Đây là dữ liệu mô phỏng được tạo cho lab, không phải log production thật. Đường dẫn được rút gọn cho dễ đọc.

| Time | Request | Tenant identity | Requested document | Status | Notes |
|---|---|---|---|---|---|
| 09:01:12 | R-100 | tenant-42 | reports/2026/summary.txt | 200 | Normal document |
| 09:02:18 | R-101 | tenant-42 | ../tenant-42-backup/confidential.txt | 200 | DLP flagged returned content |
| 09:03:04 | R-102 | tenant-42 | ../unrelated/note.txt | 403 | Rejected |
| 09:05:11 | R-103 | tenant-42 | reports/2026/summary.txt | 200 | Normal after incident |

Bổ sung từ incident commander: không thấy 401, JWT validation thành công; file server không trả 5xx; CDN hit ratio gần như không đổi. Một thay đổi vận hành gần đây tạo thư mục backup với tên gần giống tenant directory. Chưa có bằng chứng về quyền filesystem, symbolic link hoặc cache cross-tenant.

## Nhiệm vụ phân tích
- Vì sao hai request có dạng đường dẫn tương tự lại khác trạng thái?
- Dựa vào kết quả harness, hypothesis nào được hỗ trợ?
- Những tín hiệu nào là noise? Cần thêm log/metric nào trong production mà không ghi nội dung tài liệu hoặc PII?
