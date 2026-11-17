# Bài toán và ràng buộc

Ứng dụng ASP.NET Core/.NET 8 + PostgreSQL nhận bản ghi offline. Một inspection gồm `id`, `version`, `surfaceCrackMm`, `photoIds`, `closureRequired`, `notes`. Thiết bị gửi toàn bộ snapshot cùng `baseVersion` và `operationId`. Server hiện nhận snapshot cuối cùng đến, ghi đè toàn bộ record và tăng version. `receivedAt` là thời điểm gateway nhận, không phải lúc kỹ sư đo.

Invariant: không được tự động chuyển `closureRequired=true` về false nếu chưa có quyết định mở lại cầu được xác nhận. Không được làm mất ảnh bằng chứng hoặc thay đổi độc lập của thiết bị khác. Mỗi operation phải idempotent; đồng bộ lại không được phát sinh audit trùng.

SLA: 2.000 thiết bị, mất mạng tối đa 72 giờ, 20.000 operations/ngày; sau khi kết nối, 95% operations được xác nhận trong 2 phút. Nhân lực vận hành 2 backend engineers; chi phí tối đa 150 USD/tháng cho thành phần đồng bộ bổ sung. Phải lưu audit 365 ngày, PII trong ảnh phải có access control.

## Bài tập
1. Replay event theo thứ tự nhận; ghi trạng thái sau từng bước và tìm violation.
2. So sánh ít nhất 3 chính sách: last-write-wins, optimistic concurrency + manual conflict, field-level merge + safety escalation. Chỉ ra trường hợp mỗi chính sách không đủ.
3. Thiết kế request/response HTTP với `operationId`, `baseVersion`, server version và conflict payload; nêu cách tránh retry double apply.
4. Vẽ state transition: pending → applied/conflict/rejected; đề xuất nơi lưu immutable audit và bằng chứng.
5. Định nghĩa test replay cho out-of-order, duplicate operation, concurrent update cùng field, offline clock drift và unauthorized photo access.
6. Đề xuất rollout, telemetry và phương án rollback không làm mất event.

## Chấp nhận
Không có silent lost update, không phá safety invariant; xung đột không thể tự hợp nhất phải hiển thị cho người có thẩm quyền; quyết định phải giải thích chi phí/độ phức tạp và failure modes.
