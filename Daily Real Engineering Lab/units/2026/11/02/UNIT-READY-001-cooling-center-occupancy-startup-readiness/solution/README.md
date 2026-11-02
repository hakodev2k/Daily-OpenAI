# Reference Solution — spoiler

## Symptoms

Slow partner trả null trước khi snapshot sẵn sàng; failed partner bị biến thành dữ liệu trống. Fast partner che giấu vấn đề.

## Evidence

Task gate tái hiện thứ tự deterministic. Reproduce kiểm tra triệu chứng, verify kiểm tra không trả sớm, dữ liệu đúng và lỗi được truyền.

## Root cause

ReadAsync chờ _gracePeriod, chỉ là giả định về thời gian khởi tạo, không đại diện cho kết quả thực của InitializeAsync. Dữ liệu có thể chưa được công bố hoặc initialization đã fault.

## Why the fix works

Reference Program.cs chờ _initialized.Task trong ReadAsync. InitializeAsync công bố snapshot trước khi hoàn tất Task; nếu lỗi, Task fault với exception gốc. Reader không thể trả dữ liệu chưa có.

## How to verify

~~~powershell
./verify.ps1
./solution/self-check.ps1
~~~

Lệnh đầu kiểm tra starter learner sửa, lệnh thứ hai kiểm tra reference. Starter gốc phải fail verify. Harness không dùng sleep thực.

## Alternative fixes

Dùng readiness abstraction chung; hoặc hoàn tất initialization trước khi nhận request nếu startup SLA cho phép. Có thể trả 503/unknown minh bạch khi chưa sẵn sàng.

## Wrong / tempting fixes

Tăng grace period chỉ giảm xác suất; retry null vô hạn che giấu fault; lock không tự tạo dữ liệu; nuốt exception thành 0 ghế khiến người dùng quyết định sai.

## Production implications

Tách liveness khỏi readiness, thêm timeout/cancellation, telemetry, HTTP 503 hoặc stale snapshot có timestamp và freshness policy. Task completion trong process không bảo đảm durability sau restart.

## Trade-offs

Chờ snapshot tăng latency request đầu; fail-fast rõ ràng khi timeout; stale fallback cải thiện availability nhưng tăng rủi ro dữ liệu cũ. Không tự động thêm Redis.

## Senior engineer should notice

Readiness là contract về publication ordering, fault propagation và lifecycle ownership. Dùng deterministic gates để chứng minh thứ tự thay vì sleep.
