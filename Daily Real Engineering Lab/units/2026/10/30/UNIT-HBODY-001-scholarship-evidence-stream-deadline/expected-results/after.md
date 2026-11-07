# After — contract cần đạt
- FAST: body `SCHOLARSHIP-EVIDENCE-V1` chính xác, không bị cancel, không vượt ngưỡng test.
- SLOW_BODY: download bị hủy trước khi body chậm hoàn tất; elapsed dưới 1000 ms, không trả thành công giả.
- CALLER_CANCEL: caller cancellation dừng body read trước 650 ms, dù business budget là 3000 ms.
- `./verify.ps1` exit code 0; `./reproduce.ps1` có thể không còn reproduce sau khi chỉnh starter.
- Không chấp nhận giải pháp chỉ bỏ timeout, tăng timeout, hoặc thay đổi test thresholds.
- Production cần phân biệt caller cancellation, deadline exhaustion và downstream failure trong metrics/logs.
