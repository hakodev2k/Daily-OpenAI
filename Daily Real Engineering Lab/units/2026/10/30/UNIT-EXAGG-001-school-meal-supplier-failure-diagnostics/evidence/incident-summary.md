# Bằng chứng incident — dữ liệu mô phỏng

Đây là fixture deterministic, không phải log production thật.

| Source | Trạng thái | Giá trị / mã |
|---|---|---|
| orchard | Faulted | HTTP_503 |
| bakery | Succeeded | 5 items |
| dairy | Faulted | INVALID_SCHEMA |

Kỳ vọng nghiệp vụ: không publish báo cáo tổng hợp khi một nguồn bắt buộc thất bại; hai mã lỗi phải xuất hiện độc lập để phân công xử lý.

Tín hiệu thứ hai: một lần chạy chỉ có tác vụ bị hủy phải giữ được semantics hủy, không tạo false incident.

Kiểm tra bằng `run.ps1`; không dùng file này làm oracle duy nhất — harness đối chiếu hành vi code.
