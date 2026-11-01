# Hint 3 — concept
Phân biệt identity (`EventId`) với logical order (`SiteId` + `Version`). Một sự kiện đến muộn có thể hợp lệ nhưng không còn đủ mới để thay thế full snapshot. Kiểm tra conflict khi identity hoặc version bị tái sử dụng với payload khác.
