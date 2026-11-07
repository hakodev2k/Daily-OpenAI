# Before
- `./reproduce.ps1` exit 0, in `REPRODUCED`.
- Callback đã bắt đầu nhưng `Cancel()` chưa trả về khi gate đóng.
- Flush count bằng 0 trước gate, bằng 1 sau gate.
- `./verify.ps1` exit 2 vì acknowledgement không độc lập với downstream.
- Không cần đo latency tuyệt đối; gate quyết định trạng thái.
