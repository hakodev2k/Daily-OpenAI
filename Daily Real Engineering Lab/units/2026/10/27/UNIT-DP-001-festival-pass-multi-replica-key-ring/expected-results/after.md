# Sau khi sửa — learner path

- `./verify.ps1` in `ACCEPTED` cho replica B và một process A mới; script exit 0.
- Purpose `festival.admin-session.v1` phải bị từ chối với exit 21.
- Thư mục key ring khác phải bị từ chối với exit 21.
- `./reproduce.ps1` **không còn** tái hiện được lỗi ban đầu sau khi sửa; đây là kết quả mong đợi, không phải regression.
- Không dùng sleep, độ trễ hoặc network timing làm điều kiện pass.
