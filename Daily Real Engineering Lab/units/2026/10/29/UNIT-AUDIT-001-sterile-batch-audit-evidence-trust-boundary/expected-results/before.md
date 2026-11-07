# Before — trạng thái fixture hiện tại

- T0 chứa 4 events cho B-4721; T1 chứa 3.
- Receipt từ dịch vụ riêng ghi 4 events và lastSequence 204.
- Export ở cả hai thời điểm đều trả HTTP 200; không có integrity alert.
- Chỉ so sánh count có thể phát hiện fixture này, **nhưng không đủ** để phát hiện thay đổi nội dung giữ nguyên count.
- Không có chữ ký thật hoặc digest thật trong fixture; không được khẳng định đã xác minh mật mã.
