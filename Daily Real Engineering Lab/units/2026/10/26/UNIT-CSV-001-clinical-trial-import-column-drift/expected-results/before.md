# Trước khi sửa

- `./reproduce.ps1` trả exit code 0 vì phát hiện lỗi (đây là **expected reproduction**, không phải pass chức năng).
- `./run.ps1` hiển thị số record/column không nhất quán với file nghiệp vụ.
- `./verify.ps1` trả exit code 1 và chỉ ra các tình huống bị sai.
- Không kết luận từ một số cột duy nhất: phải so cả số record và giá trị từng field.
