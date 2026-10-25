# Before — Baseline

- `demo`: chunk lớn đọc thành công; chunk nhỏ có thể bị từ chối dù frame không thay đổi.
- `reproduce`: in `REPRODUCED: valid frame rejected` và exit 0.
- `verify`: exit khác 0 ở một trong các ca chunk nhỏ.
- Không dựa vào timing; lỗi deterministic do `FragmentingStream` kiểm soát kích thước chunk.
