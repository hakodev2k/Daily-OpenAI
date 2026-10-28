# Scenario — Trạm cứu hộ thú nuôi
Một gateway tiếp nhận hai webhook `POST /intake` từ đối tác; payload JSON được ghi audit để phục vụ đối soát. Sau thay đổi pipeline, audit thấy dữ liệu nhưng API trả 422. Hãy phân biệt ba khả năng: JSON không hợp lệ, payload bị sửa trên đường truyền, hoặc có sự khác biệt giữa các consumer của request body.

Fixture sử dụng dữ liệu synthetic, hai request độc lập và một stream chỉ đọc tuần tự. Không dùng dữ liệu thực hoặc token thật.
