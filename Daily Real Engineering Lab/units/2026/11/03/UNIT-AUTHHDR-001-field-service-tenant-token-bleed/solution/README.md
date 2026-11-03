# Reference Solution
**Root cause:** `HttpClient.DefaultRequestHeaders` là cấu hình dùng chung. Starter thay đổi Authorization trước mỗi SendAsync; request đồng thời có thể dùng token của tenant khác. Thay đổi default headers khi requests đang chạy không được bảo đảm an toàn.

**Fix:** gắn Authorization lên `HttpRequestMessage.Headers` riêng của mỗi request, giữ HttpClient dùng chung.

**Kiểm tra:** `dotnet run --project solution/FieldDispatch.csproj -- verify` phải in Tenant isolation PASS và exit 0. `verify.ps1` luôn chạy learner-editable starter.

**Wrong fixes:** lock quanh một phép gán không bảo vệ lifetime; tạo HttpClient cho mỗi request tăng connection churn; tuần tự hóa che lỗi và giảm throughput; HTTP 200 không xác nhận tenant.

**Trade-offs:** xử lý token rotation, không log token thật, mỗi retry cần message mới, đo cả tenant attribution.
