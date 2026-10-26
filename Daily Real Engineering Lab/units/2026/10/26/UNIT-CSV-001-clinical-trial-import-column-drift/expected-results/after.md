# Sau khi sửa

- `./verify.ps1` trả exit code 0; 5 record và 4 field/record trong fixture được bảo toàn.
- Ghi chú có dấu phân cách, dấu nháy kép, CRLF nội dung và trường rỗng đều khớp chính xác.
- Input có quoted field chưa đóng bị từ chối bằng `FormatException`.
- `./reproduce.ps1` trả exit code 1 vì triệu chứng gốc không còn tái hiện.
- Kiểm tra logic trên chính `starter/` đã chỉnh sửa; reference solution chỉ dùng để đối chiếu.
