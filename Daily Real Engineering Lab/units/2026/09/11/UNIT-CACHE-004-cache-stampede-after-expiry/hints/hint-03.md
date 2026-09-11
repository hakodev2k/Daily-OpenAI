# Hint 03

Tìm một cách để chỉ cho phép **một loader cho mỗi key** tại một thời điểm, sau đó kiểm tra cache lại sau khi giành quyền load. Mục tiêu không phải serialize toàn bộ cache mà là coalesce các miss cho cùng key.
