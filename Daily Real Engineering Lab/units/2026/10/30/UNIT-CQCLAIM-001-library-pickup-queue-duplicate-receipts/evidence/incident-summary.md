# Incident evidence — dữ liệu mô phỏng để đối chiếu
- Bối cảnh: batch cấp phiếu nhận sách liên chi nhánh, 2 worker trong một process.
- Đầu vào ví dụ: `REQ-N-101`, `REQ-S-202`; số yêu cầu = 2.
- Số phiếu phát hành: 2; số ID phân biệt: 1; số ID đầu vào không xuất hiện: 1.
- Không có exception hoặc lỗi kết nối; thời gian xử lý rất ngắn.
- Điểm đồng bộ trong harness chỉ làm lịch xen kẽ tái lập được, không phải giả định về tốc độ production.
- Đây là evidence fixture thiết kế sẵn, **không phải log của một lần chạy .NET đã được thực thi**.

Hãy dùng kết quả console thực tế của bạn để kiểm tra hoặc bác bỏ các giả thuyết.
