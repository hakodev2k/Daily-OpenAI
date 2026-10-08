# Incident evidence (local deterministic replay)
- 12 sự kiện liên tiếp được API tiếp nhận, mỗi sự kiện có sequence 1..12.
- Bản replay tiêu chuẩn chỉ bắt đầu đọc sau khi đợt ghi hoàn tất.
- Không có exception, không có timeout, và bộ nhớ ổn định.
- Dashboard đếm 12 lần gọi producer; dashboard downstream ghi nhận ít hơn.
- Consumer không ghi nhận lỗi xử lý từng sự kiện.
- Giả thuyết cạnh tranh: consumer filter sai, producer mất sự kiện, queue admission policy, hoặc completion race.
- Cần chạy `reproduce.ps1` để thu thập danh sách sequence chính xác.
