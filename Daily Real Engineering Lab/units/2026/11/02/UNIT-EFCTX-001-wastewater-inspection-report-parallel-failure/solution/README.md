# Reference Solution — chỉ xem sau khi tự thử

## 1. Symptoms

Dashboard có đủ dữ liệu nhưng luồng tổng hợp hai nhóm có thể ném `InvalidOperationException` khi hai truy vấn chồng lấn. Từng truy vấn riêng có thể thành công.

## 2. Evidence

Gate trong `checks/Program.cs` giữ truy vấn đầu tại điểm I/O, bảo đảm nhánh còn lại được khởi chạy khi nhánh đầu chưa hoàn tất. Exception nhắc tới "A second operation was started on this context instance". Vì dữ liệu seed không thay đổi và truy vấn riêng hợp lệ, giả thuyết sai dữ liệu hoặc lỗi phân loại không giải thích được bằng chứng.

## 3. Root cause

`InspectionReportService.BuildAsync` khởi tạo hai `ToListAsync` trên **cùng một `DbContext`** trước khi await. EF Core không hỗ trợ parallel operations trên cùng context instance; `AsNoTracking` không thay đổi giới hạn này. `Task.WhenAll` không làm context trở nên thread-safe.

## 4. Why the fix works

[Xem source tham chiếu](InspectionReport.cs). Mỗi nhánh truy vấn có một context riêng từ factory, mỗi context được `await using` để giải phóng sau khi hai task hoàn thành. Hai operation có thể chồng lấn mà không chia sẻ EF Core change tracker/concurrency detector. Hệ thống vẫn trả đủ hai danh sách, có thứ tự xác định.

Chỉ thay phần `InspectionReportService` trong `starter/InspectionReport.cs`; không copy cả file tham chiếu vì nó giả định các model đã có trong starter.

## 5. How to verify

Chạy `./verify.ps1` sau khi sửa starter. Harness chỉ tham chiếu `starter/Starter.csproj`, không biên dịch code trong `solution/`. Điều kiện pass: không exception khi chồng lấn, Open = `WW-101, WW-103`, Closed = `WW-102`. `./reproduce.ps1` dùng để xác nhận lỗi gốc **trước** khi sửa.

## 6. Alternative fixes

- Await truy vấn Open xong mới chạy Closed trên một context: đúng và đơn giản, giảm concurrency, có thể đủ nhanh cho dữ liệu nhỏ.
- Dùng `IDbContextFactory<TContext>` từ DI thay cho `Func<TContext>` khi tích hợp ASP.NET Core/BackgroundService: quản lý scope rõ hơn, vẫn cần dispose từng context.
- Gộp một query rồi phân nhóm in-memory: giảm số round trip nếu phù hợp kích thước dữ liệu, nhưng tải thêm dữ liệu và có thể làm mất khả năng lọc hiệu quả ở DB.

## 7. Wrong / tempting fixes

- Thêm `Task.Run` quanh mỗi truy vấn: không khiến context thread-safe, còn tăng scheduling overhead.
- Chỉ thêm `AsNoTracking`: loại tracking nhưng không cho phép concurrent operations.
- Tắt thread-safety checks của EF Core: che triệu chứng, có thể gây undefined behavior/corruption; không phải fix.
- Tăng pool size hoặc retry vô điều kiện: không sửa vòng đời resource và có thể làm trầm trọng lỗi.
- Nuốt exception rồi trả một phần báo cáo: sai contract kiểm định.

## 8. Production implications

Trong web app, một scoped `DbContext` hợp lệ cho các operation tuần tự trong request, nhưng không được chia sẻ cho nhiều nhánh song song. Với background jobs, dùng factory tạo context per unit of work. Theo dõi số connection đồng thời, timeout, load DB và cancellation. Nếu hai truy vấn cần một snapshot nhất quán, hai context độc lập không tự bảo đảm cùng transaction/snapshot; cần thiết kế transaction/isolation hoặc query hợp nhất phù hợp DB.

## 9. Trade-offs

Hai context có thể cải thiện latency khi database có năng lực song song, đổi lại thêm connection và áp lực DB. Một context tuần tự thường đơn giản hơn. `Task.WhenAll` phù hợp khi operation độc lập, có ownership riêng, và có concurrency budget. Không nên tối ưu hóa song song chỉ vì có thể.

## 10. What a Senior engineer should notice

Phải phân biệt task concurrency, resource ownership, connection pooling và transaction consistency. Kiểm thử cần tạo overlap deterministic, không phụ thuộc máy nhanh/chậm. Một fix đạt yêu cầu không chỉ tránh exception mà còn giữ đúng kết quả, cancellation, cleanup và giới hạn tải database.
