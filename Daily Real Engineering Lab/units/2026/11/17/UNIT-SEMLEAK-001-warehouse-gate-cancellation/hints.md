# Hints (mở từng mức)

1. Theo dõi điều gì xảy ra khi một request bị hủy trước khi vào vùng giới hạn.
2. So sánh số lần thành công của `WaitAsync` với số lần gọi `Release`.
3. Hãy đặt phạm vi `finally` đúng theo thời gian sở hữu permit, kể cả khi công việc ném exception.
