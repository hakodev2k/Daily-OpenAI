# Investigation guide (no spoilers)

1. Chạy reproduce và run; ghi exit code và output bốn probe.
2. So sánh request lỗi và request bình thường theo status, body, audit header.
3. Đặt giả thuyết về business handler, routing, và middleware control flow; nêu cách bác bỏ từng giả thuyết.
4. Vẽ luồng request qua conditional middleware và terminal handler.
5. Sửa nhỏ nhất trong starter; chạy verify; kiểm tra thêm negative case `/missing`.

Nếu không reproduce: kiểm tra `dotnet --info`, target `net8.0`, thư mục hiện tại và xem starter đã bị chỉnh chưa. Không dùng solution thay cho learner verification.
