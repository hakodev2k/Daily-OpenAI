# Reference Solution — chỉ đọc sau khi tự làm lab

## 1. Symptoms

`cold=[101,103]` nhưng `warmed=[101,102,103]` dù không có write và cả hai đều yêu cầu active-only.

## 2. Evidence

`Program.cs` tạo một SQLite in-memory database dùng chung qua connection đang mở. Hai `DbContext` độc lập có cùng dữ liệu; đường warmed tải toàn bộ `Enrollment` trước khi gọi `CohortReader`. Sự khác biệt xuất hiện ở object graph, không phải do dữ liệu được cập nhật.

## 3. Root cause

EF Core tracking query thực hiện **relationship navigation fix-up**: các `Enrollment` đã được tracking có cùng FK sẽ được liên kết với `Cohort.Enrollments` khi `Cohort` được materialize. `Filtered Include` không bảo đảm collection chỉ chứa phần tử thỏa điều kiện nếu context đã tracking các entity liên quan. Bộ lọc ở SQL không xóa các entity đã có trong change tracker. Đây là vấn đề correctness do trạng thái `DbContext`, khác với N+1 hoặc SQL filter sai.

## 4. Why this fix works

Trong `solution/CohortReader.cs`, `AsNoTracking()` materialize read model độc lập với các tracked entity trong scope, nên filtered navigation không bị bổ sung từ tracked graph. Hành vi `SingleAsync` vẫn giữ hợp đồng ném `InvalidOperationException` nếu cohort không tồn tại.

Thử nghiệm sau khi tự sửa: `Copy-Item ./solution/CohortReader.cs ./starter/CohortReader.cs`, sau đó `./verify.ps1`. Lệnh copy chỉ dùng **sau** khi hoàn tất bài tập.

## 5. How to verify

`./verify.ps1` chạy đúng project `starter` đã được người học chỉnh sửa. Cần PASS với cold và warmed đều `[101,103]`; đồng thời missing-cohort contract PASS. `./reproduce.ps1` chỉ dành cho bản starter ban đầu; sau fix việc reproduce thất bại là đúng.

## 6. Alternative fixes

- **Projection trực tiếp thành DTO** từ query, tránh expose tracked navigation collection. Thường là lựa chọn tốt cho read-only API; cần giữ semantics của missing cohort.
- **DbContext riêng cho read operation** khi boundary nghiệp vụ thực sự độc lập; đánh đổi chi phí scope và transaction consistency.
- **Explicit query** trực tiếp lên `Enrollments` với filter rõ ràng, đồng thời kiểm tra cohort tồn tại nếu hợp đồng API yêu cầu.
- **`ChangeTracker.Clear()`** có thể tránh fix-up nhưng làm mất toàn bộ pending tracked state; không nên là biện pháp mặc định cho request có write.

## 7. Wrong / tempting fixes

- Thêm `.Where(e => e.IsActive)` **sau** materialization có thể che triệu chứng ở đúng một consumer nhưng không sửa được collection semantics và có thể để các consumer khác dùng dữ liệu sai.
- Thêm `AsSplitQuery()` chỉ đổi hình dạng SQL; không loại bỏ relationship fix-up của tracking.
- Xóa `Enrollment` inactive khỏi DB làm sai nghiệp vụ và phá audit.
- Tăng timeout, thêm index hoặc retry không liên quan vì lỗi không đến từ độ trễ hay transaction conflict.

## 8. Production implications

Không dùng `DbContext` làm cache read model xuyên request. Đối với read-only endpoints, xác định rõ tracking policy, query projection, scope ownership và kiểm thử các đường truy vấn trước đó. Theo dõi cardinality/identity trong DTO thay vì tin collection navigation luôn phản ánh SQL filter.

## 9. Trade-offs

`AsNoTracking` không hỗ trợ update trực tiếp các entity trả về qua cùng tracker; nếu cần sửa entity, dùng explicit write workflow. Projection có thể giảm tải dữ liệu và coupling nhưng phải giữ missing-entity semantics và tránh overfetch. Scope mới giảm phụ thuộc lịch sử nhưng tăng yêu cầu thiết kế transaction boundary.

## 10. Senior engineer should notice

Test trên context sạch là chưa đủ: test thứ tự truy vấn, context đã tracking các related entity, behavior khi cohort không tồn tại và ranh giới giữa database rows với materialized navigation. Đặt correctness contract ở API boundary, không ở sự tình cờ của tracking state.

## Tham khảo

[EF Core filtered include](https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager#filtered-include) và [Tracking vs no-tracking](https://learn.microsoft.com/en-us/ef/core/querying/tracking).
