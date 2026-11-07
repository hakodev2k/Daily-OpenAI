# Trước khi sửa — starter

- `./setup.ps1`: restore/build thành công nếu có SDK 10.
- `./reproduce.ps1`: hai lần `read` ở process độc lập in `REJECTED cryptographic-validation`, sau đó script in `REPRODUCED` và exit 0.
- `./verify.ps1`: dừng ở `Cross-replica session must be accepted.` (không đạt).
- Đây là bằng chứng của harness mô phỏng, **không phải log production thực tế**.

Nếu `reproduce.ps1` thất bại với exit khác 21, kiểm tra build/SDK và output trước khi suy luận root cause.
