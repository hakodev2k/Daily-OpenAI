# Acceptance cases — thiết kế phải vượt qua

| ID | Tình huống | Điều cần chứng minh |
|---|---|---|
| AC-01 | Search trước, ngay sau và 10 phút sau khi ACK | Không có hồ sơ bị gỡ trong public response sau ACK |
| AC-02 | Nightly export cũ chạy sau khi có yêu cầu gỡ | Không tái xuất hiện dữ liệu công khai |
| AC-03 | Worker crash giữa SQL update và search cleanup | Có retry, idempotency, trạng thái quan sát được |
| AC-04 | Duplicate hoặc out-of-order deletion events | Không đảo trạng thái gỡ về trạng thái hiển thị |
| AC-05 | Redis chứa query response cũ | Không lộ dữ liệu khi cache hit |
| AC-06 | Attachment URL phát hành trước khi gỡ | Không cam kết revoke tức thời nếu kiến trúc không hỗ trợ; ACK phải phản ánh đúng |
| AC-07 | Legal hold hợp lệ | Không xoá vật lý trái retention; public visibility xử lý theo policy |
| AC-08 | Restore SQL/search từ backup cũ | Không phục vụ public traffic trước khi reconciliation hoàn tất |
| AC-09 | Mất dependency kiểm tra quyền hiển thị | Quyết định fail-open/fail-closed rõ ràng, có alert |
| AC-10 | 400 search RPS, 20 hits/request | Nêu cách benchmark p95 < 250ms và budget thêm cho kiểm tra visibility |
| AC-11 | Retry DELETE cùng tenant + record + request key | Không tạo tác dụng phụ trái ý muốn; trạng thái cuối xác định |
| AC-12 | Tenant khác dùng trùng record ID | Không chạm nhầm hoặc làm lộ dữ liệu cross-tenant |

Viết test oracle cho mỗi case: input, state trước, hành động, public observable output, metric và pass/fail. Nếu thiết kế không đáp ứng một case, nêu giới hạn và kế hoạch giảm thiểu.
