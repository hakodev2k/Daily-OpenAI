# Reference Solution — chỉ xem sau khi tự reproduce và fix

## 1. Symptoms
Lần đầu trả edition 1. Sau memory pressure, origin vẫn không đổi nhưng client trả chuỗi rỗng. Trong fixture, status lần lượt là 200 rồi 304.

## 2. Evidence
Ghi lại \`StatusCodes\` và \`ConditionalRequests\`. Sau eviction, request vẫn conditional; 304 không có response body. Giá trị rỗng xuất hiện tại client chứ không phải do origin xóa catalog.

## 3. Root cause
ETag còn sống nhưng cached representation đã bị evict. Client gửi \`If-None-Match\` dù không còn body để tái sử dụng. Origin hợp lệ trả 304 (not modified), trong khi client dùng fallback \`string.Empty\` và biến trạng thái cache không hợp lệ thành kết quả business hợp lệ giả.

## 4. Why the fix works
[CatalogClient.cs](CatalogClient.cs) chỉ phát conditional request khi **cả** ETag và body còn hiện diện. Sau eviction, request không conditional và origin trả 200 với body đầy đủ. Nếu origin bất thường trả 304 khi body không tồn tại, client fail closed bằng exception, không báo catalog rỗng.

## 5. How to verify
Thay nội dung \`starter/CatalogClient.cs\` bằng mã tham chiếu, chạy \`.\verify.ps1\` từ thư mục unit. Các assertion kiểm tra:
- Cold fetch 200 và đúng edition 1.
- Eviction phải fetch lại 200, không gửi validator.
- Cache còn nguyên có thể nhận 304 và vẫn trả edition 1.
- Origin nâng edition thì conditional request nhận 200 mới, sau đó 304 vẫn dùng edition 2.
- 304 không có body khả dụng phải bị từ chối.

## 6. Alternative fixes
Có thể lưu ETag và body trong cùng một cache entry với cùng lifetime, loại bỏ trạng thái metadata-only. Với cache phân tán, cần atomicity của cặp representation/validator và policy stale fallback. Nếu origin trả 304 bất thường, một lần retry unconditional có giới hạn là phương án khác, nhưng phải tránh vòng lặp.

## 7. Wrong / tempting fixes
- Coi \`304\` là chuỗi rỗng: che giấu lỗi dữ liệu.
- Luôn tắt conditional request: đúng về dữ liệu nhưng bỏ lợi ích băng thông, không cần thiết khi cache hợp lệ.
- Retry vô hạn mọi 304: tạo vòng lặp với origin vi phạm hợp đồng.
- Giữ ETag mãi sau eviction mà không giữ body: vẫn tái tạo lỗi.

## 8. Production implications
Theo dõi tỉ lệ 200/304, cache eviction, empty catalog và bất thường 304-without-representation. Không log nội dung catalog nhạy cảm hoặc authorization headers. Cache eviction policy và revalidation policy phải cùng hợp đồng.

## 9. Trade-offs
Tái tải 200 sau eviction tăng băng thông nhưng bảo toàn correctness. Lưu body lâu hơn tăng memory. Có thể cân bằng TTL, cache size và độ mới của dữ liệu. HTTP validator không thay thế representation.

## 10. Senior engineer should notice
Tầng transport thành công không đồng nghĩa business data đúng. Phải xác định invariant giữa cache metadata và body, có regression test cho partial eviction và abnormal upstream response, và quan sát được nguyên nhân thay vì silently returning default values.

**Lưu ý:** \`reproduce.ps1\` xác nhận bug của starter; \`verify.ps1\` kiểm tra code learner đã sửa. Mã solution không được tự động dùng bởi verify.
