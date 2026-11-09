# Dữ liệu đầu vào — snapshot 2026-11-10 (synthetic)

**Giả định:** v1 nhận JSON `{ticketId,terminalId,amountCents}`; v2 nhận `{transactionId,deviceId,amountMinor,currency}`. Không có mapping tự động an toàn giữa `ticketId` và `transactionId` vì các máy kiosk cũ có thể tái sử dụng ticketId sau khi restart. Backend lưu receipt bất biến 7 năm.

| Consumer | v1 requests/7 ngày | v2 requests/7 ngày | Offline tối đa | Khả năng triển khai | Ghi chú |
|---|---:|---:|---:|---|---|
| K01 - city mobile | 40,000 | 120,000 | 2 giờ | release mỗi tuần | đã test v2 với 99.95% receipt reconciliation |
| K02 - partner portal | 2,100 | 8,300 | 1 ngày | 2 tuần | đang dual-write ở client; chưa có report duplicate |
| K03 - rural kiosks | 8,400 | 0 | 14 ngày | firmware release 45 ngày | offline queue có thể phát lại sau reconnect |
| K04 - school passes | 0 | 0 | 30 ngày | theo học kỳ | có thể không có giao dịch trong kỳ nghỉ |
| K05 - testing client | 300 | 0 | 0 | không rõ | token chung, chưa xác định owner |

## Dữ kiện bổ sung
- Product yêu cầu sunset v1 sau 21 ngày; compliance yêu cầu **không mất giao dịch** và truy vết receipt 7 năm.
- Gateway hiện có rate limit toàn cục; chưa có dashboard traffic theo `clientId + version + status`.
- p95 latency v1 180 ms; v2 145 ms, nhưng không có bằng chứng đây là nút thắt kinh doanh.
- Mỗi ngày duy trì v1 tốn 45 USD vận hành và hỗ trợ; tổng chi phí dự kiến 21 ngày: 945 USD.
- v1 đã từng nhận request muộn 12 ngày sau khi một kiosk được đánh dấu ngừng hoạt động.
- Có thể bật lại v1 trong 30 phút **nếu code và database vẫn tương thích**; một lần xóa schema sẽ khiến rollback không khả thi.
- Đội triển khai: 2 backend, 1 SRE part-time, 1 partner manager. Không có đội 24/7.

## Các lựa chọn ban đầu
A. Tắt v1 đồng loạt T+21.
B. Duy trì v1 thêm 60 ngày, song song v2, theo dõi theo consumer.
C. Dùng gateway chuyển v1 sang v2 ngay, tắt backend v1 T+21.

## Thay đổi requirement giữa bài
Sau khi viết ADR sơ bộ, Product đưa thêm điều kiện: đối tác K03 không thể cập nhật firmware trước T+60, nhưng phải tiếp tục bán vé. Chỉnh lại decision và tiêu chí chấp nhận.

## Timeline replay
- T+0: chốt snapshot.
- T+7: K01 đạt 100% v2; K02 còn 2% v1.
- T+20: K03 mất mạng; queue có 900 giao dịch v1 chưa gửi.
- T+21: đề xuất sunset.
- T+34: K03 kết nối lại, gửi 900 giao dịch v1.
- T+50: K04 bắt đầu kỳ học mới, gửi request v1 đầu tiên sau 40 ngày.
