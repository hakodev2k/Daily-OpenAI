# Client report — dữ liệu mô phỏng

Nguồn dữ liệu: waveform `SEIS-42`, representation `ABCDEFGHIJ`, tổng 10 byte. Đây là dữ liệu giả lập phục vụ lab, không phải log production thật.

| Request | Expected status | Expected body | Ghi nhận ban đầu |
|---|---:|---|---|
| GET không Range | 200 | ABCDEFGHIJ | Tải toàn bộ bình thường |
| Range bytes=3-6 | 206 | DEFG | Client ghép dữ liệu thấy thiếu một mẫu |
| Range bytes=0-0 | 206 | A | Viewer không hiển thị mẫu |
| Range bytes=7- | 206 | HIJ | Đoạn cuối ngắn bất thường |
| Range bytes=-4 | 206 | GHIJ | Tải suffix không đủ |
| Range bytes=50-60 | 416 | rỗng | Request ngoài phạm vi bị từ chối |

Bảng mô tả **triệu chứng và kỳ vọng của client**; hãy chạy harness để lấy số liệu thực tế và kiểm tra `Content-Range` trước khi kết luận nguyên nhân. Không có yếu tố timing hay race trong fixture này.
