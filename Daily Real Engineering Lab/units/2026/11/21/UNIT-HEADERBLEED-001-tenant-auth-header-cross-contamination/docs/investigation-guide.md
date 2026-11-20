# Investigation worksheet
1. Chạy reproduce 3 lần; ghi output và exit code.
2. Vẽ interleaving A cấu hình → B cấu hình → B/A gửi.
3. Hypothesis 1: fake handler đọc sai X-Tenant; hypothesis 2: credential bị thay đổi trong shared client; hypothesis 3: đối tác cache sai.
4. Đề xuất phép thử bác bỏ từng giả thuyết.
5. Sửa starter; chạy verify; giải thích vì sao fix không cần tạo HttpClient mới cho mỗi tenant.
6. Production: đánh giá tác động nếu credential đã bị gửi sai; thiết kế telemetry không log secret.
