# Business constraints và acceptance

- Báo cáo R-204 đã ký tại T1 không được thay đổi byte nội dung hoặc hash khi đọc lại ở T2.
- Có thể áp dụng hiệu chuẩn mới cho phép đo mới; hiệu chỉnh hồi tố phải phát hành revision mới, không âm thầm ghi đè báo cáo cũ.
- Có thể tái dựng giá trị đã ký từ raw reading, calibration version, công thức và rounding policy.
- API phải phân biệt `GET /reports/R-204/revisions/1` (bất biến) với `GET /reports/R-204` (revision đang hiệu lực, kèm revision ID).
- Không dùng timestamp máy trạm làm nguồn duy nhất để xác định thứ tự nghiệp vụ.
- Mỗi sửa đổi phải có actor, reason, approval, effective interval và thời điểm ghi nhận; audit log không chứa PII thừa.
- Nếu cần correction, giữ nguyên bản cũ và liên kết `supersedesRevision`; việc công bố correction phải qua phê duyệt.
- Đề xuất migration cho dữ liệu legacy không có calibration version, không tự suy diễn lịch sử thiếu bằng chứng.
