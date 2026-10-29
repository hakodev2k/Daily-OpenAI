# Investigation guide

1. Chạy `./reproduce.ps1` và lưu output của `health`, `live`, `bodyContract`.
2. Ghi ba giả thuyết: reverse proxy loại header, SSE handler bỏ qua middleware, hoặc response lifecycle làm thay đổi khả năng gắn header.
3. Dùng một break point trong middleware trước và sau `next()`, kiểm tra `context.Response.HasStarted` cho từng route.
4. Xem `starter/FactoryDashboard.cs`; xác định vị trí đăng ký hoặc ghi header so với thời điểm endpoint flush.
5. Sửa starter, chạy `./verify.ps1`; không đổi endpoint payload hoặc status.
6. Nếu không tái hiện: kiểm tra `dotnet --info`, chạy `./setup.ps1`, xem lỗi bind port loopback, đảm bảo chưa sửa starter.
7. Thử mở rộng: thêm một endpoint JSON có body, kiểm tra hành vi và giải thích sự khác biệt với 204.

Chỉ coi bài hoàn tất khi header có trên cả hai response và nội dung SSE không bị thay đổi.
