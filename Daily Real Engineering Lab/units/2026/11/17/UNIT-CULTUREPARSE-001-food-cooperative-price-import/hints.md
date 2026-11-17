# Progressive Hints

## Hint 1
Kiểm tra một bản ghi đầu vào có thực sự giống nhau ở mọi worker không. So sánh kết quả giữa nhiều locale.

## Hint 2
Phân biệt format của đối tác với locale của process. Tìm chỗ chuyển chuỗi thành số.

## Hint 3
Một wire-format cố định không nên phụ thuộc `CurrentCulture`. Xem xét `NumberStyles` và `CultureInfo.InvariantCulture`; kiểm tra cả input không đúng hợp đồng.
