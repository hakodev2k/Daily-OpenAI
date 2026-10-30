# Before — Starter

`reproduce.ps1`: exit code 0 nếu nhận ra đúng signature lỗi; `verify.ps1`: exit code 1.

Bảng quyết định dự kiến: `atStart=NotYetValid`, `beforeExpiry=NotYetValid`, `atExpiry=NotYetValid`, `beforeStart=NotYetValid`, `invalidWindow=InvalidWindow`, `afterRewind=NotYetValid`.

Các giá trị là kỳ vọng logic từ static review, không phải log đã chạy. Nếu khác, xem `docs/investigation-guide.md`.
