# Bằng chứng — dữ liệu giả lập

Đơn vị đo: NTU (độ đục); công thức hệ thống: `publishedNTU = round(rawADC * factor + offset, 2)`; làm tròn decimal half-away-from-zero. Mọi raw reading giữ nguyên.

| Mốc | Sự kiện | Dữ liệu |
|---|---|---|
| T0 08:00 | Sensor S-9 gửi reading M-71 | rawADC=120 |
| T0 08:01 | Sensor S-9 gửi reading M-72 | rawADC=80 |
| T1 09:00 | Công bố và ký R-204 revision 1 | chứa M-71, M-72; hash H1 |
| T2 13:00 | Kỹ thuật viên cập nhật bảng cấu hình S-9 | factor từ 0.10 sang 0.12; offset giữ 0.00 |
| T2 13:05 | Người dùng đọc lại R-204 | API trả giá trị tính theo bảng cấu hình hiện tại; vẫn gắn revision 1 |

Trước T2, cấu hình S-9: `factor=0.10; offset=0.00`. Sau T2: `factor=0.12; offset=0.00`. Trong hệ thống cũ, report chỉ lưu reading IDs, không lưu giá trị đã phát hành, calibration ID hoặc payload đã ký. Không có reading mới sau T1.

## Triệu chứng từ support
- Người nhận giữ PDF T1 với hai giá trị khác API T2.
- Audit ghi thay đổi factor, nhưng không cho biết report R-204 từng dùng version nào.
- Hash H1 của PDF T1 còn lưu tại kho lưu trữ độc lập.

## Phân tích cần thực hiện
Tính hai cặp giá trị và chênh lệch, chỉ ra điều gì chứng minh dữ liệu raw không đổi. Phân biệt correction hợp lệ với đọc lại bản phát hành cũ. Không xem thay đổi factor là nguyên nhân duy nhất trước khi đối chiếu provenance.
