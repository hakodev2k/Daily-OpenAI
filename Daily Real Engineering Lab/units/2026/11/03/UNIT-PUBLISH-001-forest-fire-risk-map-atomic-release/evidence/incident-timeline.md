# Incident timeline (synthetic)

- T+00:00: release r1 gồm north/south được phục vụ.
- T+00:03: publisher bắt đầu r2.
- T+00:04: north mới đã được upload; south chưa xong.
- T+00:04: reader chụp snapshot tại checkpoint.
- T+00:05: south mới upload xong.
- T+00:06: publisher báo r2 hoàn tất.

Không có network race trong simulator: checkpoint cố định để kiểm tra giả thuyết.
