# Hint 03 — Contract vòng đời

`HttpContext` gắn với HTTP request, không phải một nguồn dữ liệu bền vững cho background work. Hãy xét việc kiểm tra và đóng gói thông tin cần thiết ở thời điểm admission, trước khi công việc được đưa vào queue.
