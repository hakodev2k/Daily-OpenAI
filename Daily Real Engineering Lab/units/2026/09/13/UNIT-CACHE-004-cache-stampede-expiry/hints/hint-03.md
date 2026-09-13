# Hint 3

Tìm một cơ chế **per-key single-flight/request coalescing**: request đầu tiên thực hiện reload; các request cùng key đang đến đồng thời await cùng một in-flight operation thay vì tự reload riêng.
