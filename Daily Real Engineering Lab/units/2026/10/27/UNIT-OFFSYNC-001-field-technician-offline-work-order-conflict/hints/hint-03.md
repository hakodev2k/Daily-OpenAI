# Hint 03 — hướng triển khai

So sánh BaseFieldRevision với revision của đúng field trước khi apply. Lưu ChangeId đã chấp nhận kèm payload để nhận diện retry nguyên vẹn và từ chối việc tái sử dụng ID với payload khác. Trong production, cả update và dedup record cần commit atomically.
