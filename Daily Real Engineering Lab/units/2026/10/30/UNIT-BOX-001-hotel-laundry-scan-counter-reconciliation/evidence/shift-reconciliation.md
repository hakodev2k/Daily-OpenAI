# Bằng chứng ca giặt là (fixture mô phỏng)

Đầu vào ca sáng: BAG-101 hợp lệ, BAG-102 hợp lệ, BAG-101 quét lặp, BAG-103 hợp lệ, BAG-104 không đọc được. Ca chiều: BAG-201 hợp lệ, BAG-201 lặp, BAG-202 không đọc được.

Kết quả dự kiến khi chạy starter ban đầu:

~~~text
morning: processed=3 worker=3 report=0
afternoon: processed=1 worker=1 report=0
empty: processed=0 worker=0 report=0
REPRODUCED: worker totals and end-of-shift reports disagree.
~~~

Các giả thuyết ban đầu có thể bao gồm: quy tắc loại trùng sai, bộ lọc mã không đọc được sai, lỗi ghi nhận trạng thái, hoặc phép tổng hợp cuối ca sai. Hãy phân biệt chúng bằng số liệu trước khi sửa.

Lưu ý: đây là bằng chứng từ fixture được thiết kế, không phải log production thật.
