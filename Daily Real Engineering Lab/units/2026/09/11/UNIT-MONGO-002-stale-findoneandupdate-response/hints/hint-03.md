# Hint 3

`FindOneAndUpdateOptions<TDocument>` có option quyết định trả document **trước** hay **sau** update. Mục tiêu là lấy post-update document ngay từ cùng atomic operation, không gọi thêm `Find`.