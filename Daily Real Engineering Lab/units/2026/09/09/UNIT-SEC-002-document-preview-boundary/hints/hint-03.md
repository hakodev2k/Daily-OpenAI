# Hint 03

Một cách sửa nhỏ là canonicalize allowed root, loại trailing separator dư thừa, rồi so sánh target với `allowedRoot + Path.DirectorySeparatorChar` bằng comparison semantics phù hợp với platform/contract của lab. Mục tiêu là chứng minh target nằm **bên dưới directory boundary**, không chỉ bắt đầu bằng cùng chuỗi ký tự.
