# Verification — policy contract

Ghi test, nguồn evidence và tiêu chí pass/fail cho từng mục:

| ID | Input / failure injection | Contract phải chứng minh |
|---|---|---|
| A1 | ClosedAt chưa đủ 30 ngày | Không xóa trước hạn |
| A2 | Active hold trên claim đã hết retention | Không xóa sau khi hold ACCEPTED |
| A3 | Hold release sau retention boundary | EligibleAt/deadlineAt tính lại đúng |
| A4 | Hai worker xử lý cùng message | Không ghi đè trạng thái mới, audit đầy đủ |
| A5 | Delete timeout sau khi object thực sự đã mất | Retry idempotent và hội tụ |
| A6 | DB DELETED nhưng blob còn | Phát hiện orphan, có cleanup |
| A7 | DB ACTIVE nhưng blob mất | Phát hiện integrity incident, không trả AVAILABLE |
| A8 | Hold đến sau queue nhưng trước delete | Hợp đồng ACK và xử lý race rõ ràng |
| A9 | Worker crash sau storage delete trước DB receipt | Phục hồi bằng durable state |
| A10 | Restore backup cũ chứa object đã xóa | Không tái công bố sai policy |
| A11 | Message đảo thứ tự / clock skew | Không state regression |
| A12 | Quá hạn 72h | Alert, audit và escalation |

Không cần chạy cloud. Pass khi ADR có cơ chế cụ thể và testable cho tất cả checks, không chỉ nêu tên dịch vụ.
