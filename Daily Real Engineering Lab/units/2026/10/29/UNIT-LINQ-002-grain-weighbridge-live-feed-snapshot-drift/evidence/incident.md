# Incident evidence — dữ liệu synthetic

| Thời điểm | Event | Phiếu hiện diện |
|---|---|---|
| 09:00:00 | Gateway phục vụ snapshot S1 | G-101: 100 kg, G-102: 200 kg |
| 09:00:01 | Nhập kho nhận thêm G-103 | G-101: 100 kg, G-102: 200 kg, G-103: 400 kg |
| 09:00:02 | Báo cáo hoàn thành | ReceiptCount=2; TotalWeightKg=700; AverageWeightKg=350 |

Thông tin phụ: CPU thấp, không có exception, mọi trọng lượng dùng `decimal`. Gateway có counter `SnapshotOpens` để xác định số lần đọc nguồn. Hãy tìm bằng chứng nào liên quan trực tiếp đến mâu thuẫn, bằng chứng nào chỉ là nhiễu.
