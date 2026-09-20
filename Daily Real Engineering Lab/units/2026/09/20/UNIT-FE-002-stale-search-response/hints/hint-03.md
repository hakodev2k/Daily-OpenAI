# Hint 3

Một cách xử lý là gắn monotonically increasing generation/request id cho mỗi intent và chỉ cho generation hiện tại commit state. Cancellation cũng hữu ích, nhưng vẫn nên suy nghĩ về commit boundary.