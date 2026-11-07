# Reference Solution — SPOILER

## Symptom
Hai client đọc cùng version. Client A cập nhật thành công; client B vẫn có thể gửi state cũ và nhận success, khiến một phần thay đổi của A bị mất.

## Evidence
Starter in ra hai read cùng `Version=1`, sau đó cả hai write đều `200 OK`. State cuối phản ánh payload stale của B thay vì bảo toàn thay đổi mới hơn của A.

## Root cause
API write contract không mang precondition gắn với representation mà client đã đọc. `Version` tồn tại trong data nhưng server không dùng nó để quyết định request còn hợp lệ hay không. Đây là lost-update boundary problem, không phải vấn đề thread safety nội bộ đơn thuần.

## Fix
Server phát hành resource version dưới dạng `ETag`. Client gửi `If-Match` khi update. Server chỉ thực hiện write nếu validator còn khớp current representation; stale validator trả `412 Precondition Failed`.

## Why this works
Write trở thành compare-before-mutate tại resource boundary. Một client không thể âm thầm áp dụng quyết định dựa trên state cũ sau khi resource đã thay đổi.

## Verification
Chạy hai client từ cùng version. Write A phải thành công. Write B với validator cũ phải bị reject và state sau cùng phải giữ thay đổi của A.

## Wrong fixes
- `lock` trong một process: không tạo distributed/API concurrency contract và không giúp client nhận biết stale state.
- Last-write-wins: giữ symptom thay vì giải quyết nó.
- Chỉ trả `ETag` nhưng không yêu cầu `If-Match`: version trở thành metadata vô nghĩa cho write safety.
- Retry stale request tự động: có thể tiếp tục áp dụng intent dựa trên state cũ.

## Alternatives
Database row version/concurrency token vẫn nên bảo vệ persistence boundary. Trong API thực tế, HTTP precondition và database optimistic concurrency thường phối hợp: HTTP thể hiện client contract, database token bảo vệ race ở storage.

## Production implications
Xác định rõ strong/weak validator, mapping giữa ETag và persistence version, semantics của PATCH/PUT, response khi conflict, cache/proxy behavior và cách client refresh/merge sau `412`.

## Trade-offs
Optimistic concurrency làm client flow phức tạp hơn và conflict trở thành trạng thái nghiệp vụ phải xử lý. Đổi lại, hệ thống tránh silent data loss mà không cần pessimistic locking dài hạn.

## Senior insight
Concurrency correctness cần được bảo vệ ở đúng boundary. Một database token chỉ nằm bên trong repository chưa đủ nếu public API cho phép client gửi stale intent mà không có precondition rõ ràng.
