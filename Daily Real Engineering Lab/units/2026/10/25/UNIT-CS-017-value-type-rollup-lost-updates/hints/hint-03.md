# Hint 3 — Cơ chế
`WorkSummary` là `struct`. `TryGetValue` đưa một **bản sao giá trị** vào biến `out`. Sau khi thay đổi bản sao, cần xác định bước nào sẽ cập nhật lại entry trong dictionary. Không đổi `struct` thành `class` chỉ để test pass: điều đó thay đổi contract snapshot.
