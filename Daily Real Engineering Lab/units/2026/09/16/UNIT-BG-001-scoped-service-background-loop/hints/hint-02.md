# Hint 2
`BackgroundService`/hosted worker thường sống lâu. Kiểm tra dependency nào đang bị giữ sống cùng worker dù semantics của nó phù hợp hơn với một unit of work ngắn.