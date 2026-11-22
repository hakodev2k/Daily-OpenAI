# Điều tra theo bằng chứng

1. Ghi số bản ghi đầu vào và log accepted của từng lần enqueue.
2. Kiểm tra các thuộc tính mà comparer dùng và giả thuyết trùng khóa.
3. Đối chiếu API collection khác để loại trừ lỗi input.
4. Viết invariant: mỗi RequestId duy nhất phải xuất hiện đúng một lần; thứ tự priority không đổi.
5. Chạy `verify.ps1` sau sửa; bổ sung thử nghiệm cho priority bằng nhau và khác nhau.
