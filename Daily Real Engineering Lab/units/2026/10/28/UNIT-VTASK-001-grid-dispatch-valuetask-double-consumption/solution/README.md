# Reference Solution — spoiler

**Chỉ đọc sau khi đã reproduce và thử fix trong `starter/TelemetryDispatch.cs`.**

## Symptoms
Gateway trả reading hợp lệ, nhưng dispatcher ném `InvalidOperationException` và không tạo audit/receipt.

## Evidence
Baseline: một gateway request, zero audit event; exception xảy ra trong đoạn xử lý sau khi lấy reading. Không có bằng chứng gateway timeout.

## Root cause
Code giữ `ValueTask<int>` trong biến `pending`, rồi `await pending` hai lần: lần đầu lấy reading cho receipt, lần hai lấy reading cho audit. Fixture sử dụng `IValueTaskSource<int>` với hợp đồng consume một lần; lần hai ném exception. Không được suy ra rằng mọi `ValueTask<T>` đều await nhiều lần an toàn chỉ vì một số implementation dùng `Task<T>` hoặc giá trị trực tiếp.

## Why the fix works
Await một lần và lưu `int validatedKilowatts`; sử dụng lại *giá trị* cho audit và receipt. Không gọi lại gateway và không cần convert sang `Task`.

## How to verify
Chép phần triển khai reference vào `starter/TelemetryDispatch.cs` trong bản clone thực hành rồi chạy `./verify.ps1`. Bộ kiểm thử xác nhận 2 receipts, 2 audit events đúng thứ tự và đúng 2 gateway requests. Không chỉnh `Program.cs` hay `SignalClient.cs`.

## Alternative fixes
- `AsTask()` một lần trước khi chia sẻ nhiều consumer có thể phù hợp khi thực sự cần fan-out asynchronous; nhưng tạo/giữ Task không cần thiết cho pipeline tuần tự này.
- Đổi hợp đồng API thành `Task<int>` nếu nhiều consumers độc lập cần await nhiều lần, với trade-off allocation và implementation.
- Tách gateway acquisition khỏi audit pipeline nếu có yêu cầu durable audit; khi đó phải thiết kế transactional/outbox boundary riêng, không chỉ sửa await.

## Wrong / tempting fixes
- Gọi gateway lần hai để lấy số cho audit: có thể sinh reading khác và làm tăng I/O; test request-count bắt lỗi này.
- Catch `InvalidOperationException` rồi bỏ qua audit: che triệu chứng, vi phạm yêu cầu audit.
- Dùng `ConfigureAwait(false)` trên cùng `ValueTask`: thay đổi context continuation, không sửa ownership/consumption contract.
- Giả định `ValueTask<T>` luôn await nhiều lần: có thể chạy với một provider nhưng lỗi khi đổi sang pooled `IValueTaskSource<T>`.

## Production implications
Hợp đồng async result cần được xem như ownership contract. Khi tối ưu allocation bằng pooled sources, thêm regression tests cho consumer semantics. Audit trong fixture chỉ là in-memory; production cần durable storage, observability và xử lý lỗi audit theo business requirement.

## Trade-offs
`Task<T>` dễ compose/chia sẻ hơn; `ValueTask<T>` có thể giảm allocation ở đường hot path nhưng tăng độ phức tạp sử dụng và kiểm thử. Không đổi sang `ValueTask` chỉ vì kỳ vọng performance nếu chưa đo.

## Senior engineer should notice
Phân biệt số lần gọi downstream và số lần consume cùng một kết quả; tránh workaround tạo thêm side effects; đặt contract test trên provider có semantics chặt thay vì chỉ dùng `Task.FromResult`.

## References
- Microsoft Learn: https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1
- Microsoft Learn: https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.sources.ivaluetasksource-1
- .NET performance considerations: https://learn.microsoft.com/en-us/dotnet/standard/async-in-depth
