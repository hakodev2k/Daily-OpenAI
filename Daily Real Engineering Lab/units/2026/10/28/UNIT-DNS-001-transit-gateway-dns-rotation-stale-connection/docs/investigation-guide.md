# Investigation Guide (không spoil)
1. Chạy inspect hai lần trong process riêng, ghi response và connection count.
2. Tạo bảng so sánh: DNS/route lookup, TCP connect, HTTP request, HTTP response. Phân biệt lifecycle.
3. Thử thay đổi delay giữa cutover và request #2; quan sát liệu chờ lâu có đủ khi connection vẫn active.
4. Sau khi sửa, verify hai điều kiện: đúng destination và không regression request #1.
5. Phân tích khi upstream cũ đã ngừng hoạt động: timeout, retry budget và việc retry có làm mới connection hay không.
