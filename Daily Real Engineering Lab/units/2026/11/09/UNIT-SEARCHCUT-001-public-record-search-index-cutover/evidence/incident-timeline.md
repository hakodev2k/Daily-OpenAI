# Evidence pack — snapshot diễn tập (giờ địa phương)

| Thời điểm | Sự kiện | index-v1 | index-v2 | Ghi chú |
|---|---|---|---|---|
| 08:59 | baseline | 2,400,000 docs, seq 500 | chưa tạo | API đọc v1 |
| 09:00 | snapshot DB bắt đầu | 2,400,000, seq 500 | đang build snapshot seq 500 | DB vẫn nhận ghi |
| 09:03 | DB commit seq 501–503 | 2,400,002, seq 503 | 1,600,000, seq 500 | 2 insert + 1 update |
| 09:05 | build snapshot hoàn tất | 2,400,002, seq 503 | 2,400,000, seq 500 | cluster health green |
| 09:06 | thử chuyển read sang v2 | 2,400,002, seq 503 | 2,400,000, seq 500 | 5xx=0%, p95=80ms |
| 09:07 | người dùng phản ánh | 2,400,002, seq 503 | 2,400,000, seq 500 | hồ sơ R-9001 thiếu, R-0420 cũ |

## Giới hạn vận hành
- Search availability >=99.9% trong cửa sổ; p95 < 250ms; rollback quyết định trong 5 phút.
- Rebuild 5 phút; cập nhật DB tiếp tục, không chấp nhận pause writes quá 10 giây.
- Không được trả bản ghi bị gỡ công khai sau khi quyết định gỡ đã có hiệu lực; độ trễ index thông thường <=30 giây.
- Team 3 backend engineers; không có ngân sách chạy dual index quá 48 giờ.
- Index v2 thay analyzer nên so sánh thứ tự ranking tuyệt đối không phải điều kiện bắt buộc.
- Không giả định event bus exactly-once; change stream có thể retry và đến trễ.
- Một cảnh báo disk 68% xuất hiện lúc 09:02, chưa có bằng chứng tác động đến kết quả.
