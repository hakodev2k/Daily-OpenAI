# Incident evidence — 2026-10-30

## Report của vận hành
- 14:03: khách dùng trình duyệt tiếng Việt mở thông báo đóng cửa.
- 14:04: khách dùng trình duyệt tiếng Anh nhận cùng nội dung tiếng Việt.
- Dashboard: HTTP 200 ổn định, lỗi 5xx = 0, median latency giảm sau đợt tối ưu.
- Dịch vụ không có user session cho endpoint công khai này.

## Dữ liệu cần thu thập trong lab
| Request | Path | Accept-Language | Status | language | renderCount |
|---|---|---|---|---|---|
| A | /api/pool/closure | vi-VN | 200 | ghi lại | ghi lại |
| B | /api/pool/closure | en-US | 200 | ghi lại | ghi lại |
| C | /api/pool/closure | vi-VN | 200 | ghi lại | ghi lại |
| D | /api/pool/closure | en-US | 200 | ghi lại | ghi lại |

Giữ các phép thử độc lập: mỗi lần chạy script tạo process và cổng riêng. Các số trên là tình huống nghiệp vụ, không phải log thực đo trên máy của bạn.
