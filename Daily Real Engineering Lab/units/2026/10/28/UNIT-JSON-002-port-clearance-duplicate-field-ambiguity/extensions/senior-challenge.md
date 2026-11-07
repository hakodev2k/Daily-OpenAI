# Senior Challenge (optional)

Giả sử một đối tác gửi JSON nested với hàng nghìn field và cần streaming ingestion ở 1.000 requests/s. Đề xuất chính sách logical-key uniqueness cho từng object scope, giới hạn depth/size, chiến lược HTTP 400 và metrics theo partner không lộ payload. So sánh DOM validation và streaming validation về allocation, complexity và threat model. Không cần triển khai để hoàn thành lab chính.
