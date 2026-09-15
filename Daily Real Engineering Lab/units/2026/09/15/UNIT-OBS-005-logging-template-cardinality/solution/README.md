# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## Symptoms

Workload ghi đúng 20 event nhưng số message template tăng cùng số `OrderId` khác nhau.

## Evidence

Starter báo `EVENTS=20` và `TEMPLATES=20`. Đây là dấu hiệu cấu trúc event thay đổi theo business value thay vì chỉ dữ liệu event thay đổi.

## Root cause

`OrderId` được interpolate trực tiếp vào chuỗi trước khi chuỗi được chuyển cho logging boundary. Vì vậy mỗi giá trị tạo một template/signature riêng.

## Why the fix works

Giữ template ổn định (`Order {OrderId} loaded successfully`) và truyền `OrderId` như structured property. Workload vẫn có đủ context để query nhưng chỉ có một loại template.

## How to verify

Áp dụng cùng nguyên tắc vào `starter/Program.cs`, sau đó chạy `./verify.ps1`. Script yêu cầu đủ 20 event và không quá 2 template.

## Alternative fixes

Một logging abstraction có API typed/event-specific cũng có thể bảo đảm template ổn định. Source-generated logging (`LoggerMessage`) là lựa chọn tốt trong ứng dụng .NET thực khi phù hợp.

## Wrong or misleading fixes

- Xóa `OrderId` khỏi log làm giảm cardinality nhưng mất diagnostic context.
- Chỉ tăng quota/index capacity không sửa telemetry contract.
- Gom log bằng regex downstream làm tăng operational complexity và không ngăn dữ liệu cardinality cao được ingest.

## Production implications

Dynamic templates làm giảm khả năng aggregate theo event type, tăng số group/index term và có thể tăng chi phí telemetry. Structured fields giúp query theo business context mà không biến mỗi value thành một event shape mới.

## Trade-offs

Structured logging vẫn cần kiểm soát cardinality của dimensions/tags ở backend metrics và telemetry. Không phải mọi property đều phù hợp làm indexed dimension.

## What a Senior engineer should notice

Logging là data contract. Message template nên mô tả loại event; dữ liệu thay đổi theo request nên được giữ ở structured properties với policy rõ ràng về PII, indexing và cardinality.
