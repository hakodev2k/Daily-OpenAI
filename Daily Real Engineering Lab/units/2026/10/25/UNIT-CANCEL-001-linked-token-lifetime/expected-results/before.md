# Before — starter chưa sửa

Chạy `reproduce.ps1` trên starter nguyên bản:
- Hai dòng `caller` và `host` có `scannerCalled=True`.
- Cả hai có `downstreamTokenCancelled=False`, `taskCancelled=False`, `pendingBeforeRelease=True`.
- Dòng cuối: `EXPECTED_REPRODUCED`; exit code 0 nghĩa là tái hiện thành công.
- `verify.ps1` trên starter chưa sửa phải in `FAILED: ... Cancellation contract failed ...` và trả exit code khác 0.

Nếu không thấy triệu chứng, kiểm tra starter có bị sửa trước đó hay không; không dựa vào số mili-giây chính xác.
