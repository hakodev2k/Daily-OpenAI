# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms
Một dashboard request của tenant south trả work order north, không có exception. Tenant không có bản ghi cũng nhận work order north. Đây là sự cố **cross-tenant data exposure**, không chỉ là UI hiển thị sai.

## 2. Evidence
Fixture giữ một SQLite connection in-memory, seed ba work orders, sau đó chạy bốn request tuần tự. Database seed đúng, query nhanh và lỗi phụ thuộc thứ tự tenant. Khi poolSize=1, một WorkOrdersContext có thể được borrow nhiều lần.

## 3. Root cause
EF Core DbContext pooling reset state nội bộ do EF quản lý, nhưng không tự reset property ứng dụng **WorkOrdersContext.TenantId**. Starter chỉ gán property nếu null. Sau request north, instance quay về pool vẫn giữ giá trị north; request south borrow lại instance đó và global query filter vẫn sử dụng north. **AsNoTracking không thay đổi tenant filter**.

## 4. Why the fix works
Gán **db.TenantId = tenantId** ở mọi lần borrow, trước query. Filter trong Model.OnModelCreating tham chiếu property của context hiện tại nên EF Core lấy giá trị mới cho từng lần thực thi. Sửa đúng file **starter/WorkOrderReader.cs**; không sửa seed hoặc expected assertions.

## 5. How to verify
Chạy **./verify.ps1** trên starter đã sửa. Bốn assertions phải pass, exit code 0. Chạy thêm đảo thứ tự tenant và tenant rỗng. Có thể so sánh bằng cách thay nội dung starter/WorkOrderReader.cs bằng solution/WorkOrderReader.cs (chỉ sau khi đã tự thử).

## 6. Alternative fixes
- **Scoped wrapper quanh PooledDbContextFactory**: wrapper nhận tenant từ authenticated request context và luôn gán tenant mỗi lần CreateDbContext; hữu ích để tránh nhiều call site quên gán.
- **Không dùng pooling**: mỗi DbContext mới có vòng đời rõ ràng hơn; có chi phí cấp phát/khởi tạo cao hơn, cần benchmark trước khi quyết định.
- **Database Row-Level Security** nếu engine hỗ trợ: defense in depth; không thay thế kiểm soát identity và session context đúng cách.

## 7. Wrong / tempting fixes
- **AsNoTracking**: chỉ thay change tracking, không reset custom TenantId.
- **ChangeTracker.Clear**: dọn tracked entities, không reset property ứng dụng.
- **Tăng poolSize**: có thể làm lỗi ít tái hiện hơn nhưng không bảo đảm isolation.
- **Restart process**: xóa symptom tạm thời; request đầu của tenant khác có thể lại gây leak.
- **Bỏ query filter**: loại bỏ một lớp cô lập dữ liệu, tăng nguy cơ rò rỉ.

## 8. Production implications
Không lấy tenantId từ header do client tự khai báo mà không xác thực. Tenant identity phải được suy ra từ authenticated principal hoặc một boundary đã kiểm chứng. Cần regression tests đổi tenant tuần tự và đồng thời; log tenant scope và correlation ID nhưng không log dữ liệu nhạy cảm. Với pool, mọi mutable per-request state phải có chính sách khởi tạo/cleanup rõ ràng.

## 9. Trade-offs
Pooling có thể giảm allocation và initialization overhead nhưng làm vòng đời context khác với giả định “new instance per request”. Wrapper có thêm abstraction nhưng tập trung enforcement. Không pooling đơn giản hơn; chỉ nên giữ pooling nếu benchmark và isolation tests chứng minh lợi ích.

## 10. What a Senior engineer should notice
Đây là **data isolation incident**: ưu tiên chặn truy cập sai tenant, xác định phạm vi ảnh hưởng, điều tra audit logs, cân nhắc thông báo sự cố theo chính sách tổ chức, sau đó mới tối ưu. Kiểm thử happy path một tenant không chứng minh an toàn multi-tenant. Thử nghiệm cần có tenant rỗng và thay đổi thứ tự request để bắt state leak.

## Static-review limitation
Bộ source và scripts được đối chiếu tĩnh; chưa chạy dotnet restore/build/tests khi tạo lab. Learner cần chạy reproduction và verification trên máy có .NET SDK 8 và NuGet.
