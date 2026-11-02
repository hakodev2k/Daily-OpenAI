# Before — starter
`./scripts/run.ps1`: `Incoming=5; accepted=3; retained=3`. Snapshot chỉ có `OX-101`, `OX-201`, `OX-301`. `OX-102` và `OX-103` biến mất.
`./scripts/reproduce.ps1` exit 0 và in `REPRODUCED`. `./scripts/verify.ps1` exit 1; ba contract đầu FAIL, duplicate contract cũng FAIL vì snapshot không có đủ năm sự kiện.
