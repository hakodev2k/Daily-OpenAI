# Reference Solution — Chỉ xem sau khi tự điều tra

## 1. Symptoms
Các batch parallel tạo đủ audit rows nhưng nhiều row có AuditTileId không bằng TileId. Case một tile vẫn đúng.

## 2. Evidence
Batch gate bảo đảm tất cả nhánh đã chuẩn bị xong trước khi ghi audit. Do đó mismatch không phải do thiếu rows hay sai thứ tự Task.WhenAll. Mỗi nhánh đọc cùng một giá trị TileId cuối cùng trong object ambient.

## 3. Root cause
AsyncLocal<TraceState> lưu **tham chiếu** đến TraceState. Khi một execution context được flow sang các async child, bản thân object TraceState không được deep-clone. Starter thay đổi thuộc tính TileId trên cùng một instance mutable. Các nhánh ghi đè state của nhau.

## 4. Why the fix works
Reference tạo TraceState mới và **gán** vào AsyncLocal.Value bên trong từng nhánh. Gán mới là thao tác riêng theo execution context; sau các await, mỗi nhánh đọc object của mình. Không cần thay đổi batch gate.

Thay đổi trọng tâm nằm trong [TraceLab/TileAuditPipeline.cs](TraceLab/TileAuditPipeline.cs).

## 5. How to verify
Chạy dotnet run --project solution/TraceLab/TraceLab.csproj -- verify để kiểm tra reference. Sau đó tự áp dụng giải pháp vào starter và chạy ./verify.ps1. Reproduce gate chỉ dùng để xác nhận lỗi gốc.

## 6. Alternative fixes
- Truyền tileId như tham số tường minh tới audit writer: rõ data flow nhất, dễ unit test, giảm coupling vào ambient state.
- Dùng immutable record làm context và gán instance mới ở mỗi nhánh: phù hợp nếu cần ambient tracing nhiều lớp.
- Scoped immutable context theo một work item: phù hợp khi framework đã có request/job scope, nhưng cần quản lý lifetime.

## 7. Wrong or misleading fixes
- Thêm Task.Delay: chỉ thay đổi lịch scheduling, không bảo đảm isolation.
- Thêm lock quanh mutation: có thể tránh một số interleaving nhưng các nhánh vẫn đọc chung state khi rời lock; muốn khóa cả pipeline sẽ phá parallelism.
- Chỉ dùng ConcurrentDictionary cho audit rows: collection thread-safe không làm correlation context đúng.
- Bỏ batch gate: có thể che triệu chứng nhưng phá contract xử lý batch và không loại bỏ aliasing.

## 8. Production implications
Correlation sai có thể làm hỏng audit trail và điều tra sự cố dù dữ liệu nghiệp vụ khác vẫn đúng. Khi dùng AsyncLocal với HttpContext, logging scopes hoặc tracing, cần phân biệt propagation của value với ownership của mutable payload.

## 9. Trade-offs
Explicit context parameters giúp dễ hiểu và kiểm thử nhưng thêm plumbing. Ambient context thuận tiện cho cross-cutting concerns, đổi lại khó kiểm soát khi fan-out. Immutable object tạo thêm allocation nhỏ, thường hợp lý hơn so với audit sai.

## 10. Senior engineer should notice
Một test chỉ kiểm tra count hoặc test single-tile không phát hiện lỗi. Hãy viết invariant per-row và kiểm tra fan-out đồng bộ, phân biệt concurrency safety của container với ownership của dữ liệu trong container.

## References
- Microsoft Learn: https://learn.microsoft.com/en-us/dotnet/api/system.threading.asynclocal-1
- Microsoft Learn: https://learn.microsoft.com/en-us/dotnet/api/system.threading.executioncontext
- Microsoft Learn: https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1
