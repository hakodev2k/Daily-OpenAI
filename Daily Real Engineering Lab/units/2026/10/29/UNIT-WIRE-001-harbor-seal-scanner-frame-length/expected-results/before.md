# Before — starter chưa sửa
- `./reproduce.ps1` exit 0 khi quan sát được frame hợp lệ bị từ chối.
- Log `wire=0004`, `actual=4`, nhưng `declared` không khớp và gateway báo vượt giới hạn.
- `./verify.ps1` exit khác 0 vì ít nhất một positive wire contract thất bại.
- Không có network I/O hay timing ngẫu nhiên; các kết quả là deterministic.
