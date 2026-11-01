# Khái niệm sau lab

Một snapshot cần được **chuẩn bị** trước khi **công bố**. Hai bất biến độc lập: reader không thấy dữ liệu chưa hoàn chỉnh; lỗi trước commit không phá bản hợp lệ cuối cùng.

Thay thế file cùng filesystem thường tạo một điểm công bố thống nhất. Điều đó không tự động bảo đảm dữ liệu bền vững sau mất điện: còn phụ thuộc flush dữ liệu, metadata thư mục, OS và filesystem. Network share/object storage có thể có semantics khác. Hai writer cạnh tranh vẫn cần version/lock. Đo lỗi rename, giám sát file tạm và bảo vệ last-known-good.
