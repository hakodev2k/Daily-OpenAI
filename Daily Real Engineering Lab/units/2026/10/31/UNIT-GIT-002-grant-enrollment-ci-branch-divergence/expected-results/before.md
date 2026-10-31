# Before — triệu chứng

- case-a: có file của feature và file chỉ sửa trên main.
- case-b: có file feature sửa và file chỉ thêm trên main.
- case-c: feature chưa có commit mới nhưng script liệt kê file của main.
- `reproduce.ps1` PASS khi phát hiện đúng triệu chứng; `verify.ps1` FAIL với starter ban đầu.

Không dùng timing, so sánh tập file chính xác.
