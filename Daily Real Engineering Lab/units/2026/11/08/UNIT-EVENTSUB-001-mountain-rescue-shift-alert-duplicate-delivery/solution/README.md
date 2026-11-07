# Reference solution — spoiler

Root cause: `BeginShift` đăng ký thêm delegate cho mỗi ca nhưng không gỡ delegate của ca trước. Event trên `AlertHub` giữ tham chiếu tới các callback, nên một `Publish` gọi tất cả các handler còn sống. Với ba ca, tổng số callback là 1+2+3 = 6.

Reference lưu delegate hiện hành trong `_activeHandler`, hủy đăng ký trước khi thay ca và đăng ký delegate mới. Như vậy chỉ ca hiện hành nhận tin. Với lifecycle kết thúc hoàn toàn, cần thiết kế thêm `IDisposable`/unsubscribe lúc shutdown; cân nhắc thread safety nếu bàn giao và publish chạy đồng thời.

**Wrong fixes:** (1) `Distinct()` trên delivery sau phát chỉ che duplicate và không ngăn ca cũ nhận thông tin. (2) Khởi tạo lại `AlertHub` mỗi ca phá các subscriber độc lập. (3) `GC.Collect()` không giải phóng delegate còn được event tham chiếu.

**Trade-off:** nếu nhiều nhóm cần cùng nhận tin, phải mô hình hóa subscription theo owner và dispose theo owner, không áp dụng giới hạn một handler toàn cục. Thử thêm ca D và kiểm tra chính sách khi `sink` throw.

**Reference self-check:** sao chép `solution/Program.cs` sang một thư mục project thử nghiệm riêng (không ghi đè starter) rồi chạy `dotnet run -- --verify`.
