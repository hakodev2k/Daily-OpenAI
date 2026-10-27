# Reference Solution — CHỈ XEM SAU KHI TỰ THỬ

## 1. Symptoms
Worker bắt đầu warmup, nhưng `IHost.StartAsync` chưa trả về trong lúc gate đóng. Startup probe chờ quá lâu dù không có network hay database lỗi.

## 2. Evidence
Starter xuất `host-startup-before-release=False`; sau khi gate mở, startup hoàn tất. Việc host chờ gắn trực tiếp với tiến độ worker, không phải với độ trễ của probe.

## 3. Root cause
Trên .NET 8, `BackgroundService.StartAsync` gọi `ExecuteAsync`. Phần thân của async method chạy **đồng bộ tới await đầu tiên thực sự chưa hoàn tất**. `LoadBlocking` đứng trước await đầu tiên, nên chính `StartAsync` bị giữ trong phần chạy đồng bộ đó. Đây là contract của host lifecycle, không phải do `Task.Delay` ở cuối.

**Version note:** Hành vi startup của `BackgroundService.ExecuteAsync` thay đổi từ .NET 10; lab cố ý pin `net8.0` và SDK 8 để tái hiện contract này. Không suy rộng kết quả sang mọi runtime.

## 4. Why the fix works
Trong `solution/ShelterFeedWarmupService.cs`, `await Task.Yield()` tạo một điểm nhường bất đồng bộ trước warmup. `ExecuteAsync` trả về một Task chưa hoàn tất, cho phép `BackgroundService.StartAsync` và host tiếp tục. Warmup vẫn chạy sau đó; harness kiểm tra nó không bị bỏ qua.

Áp dụng: copy **nội dung** file reference vào `starter/ShelterFeedWarmupService.cs`, sau đó chạy `./verify.ps1`.

## 5. Verification
`verify.ps1` chạy chính `starter/`, không chạy reference source. Kỳ vọng `host-startup-before-release=True`, `warmup-completed`, `host-stopped`, exit 0. `reproduce.ps1` có thể exit 1 sau fix vì không còn triệu chứng cũ.

## 6. Alternative fixes
- Nếu warmup là I/O, ưu tiên API async thật, `await` đúng điểm I/O và truyền `stoppingToken`.
- Nếu là CPU-bound nặng, cân nhắc tách bounded work sang dedicated worker/queue và kiểm soát capacity; `Task.Yield` chỉ giải quyết **startup coupling**, không làm CPU work biến mất.
- Nếu hệ thống **phải** chờ snapshot trước khi phục vụ traffic, dùng readiness riêng (không chỉ liveness/startup) hoặc thiết kế `IHostedLifecycleService`/bootstrap rõ ràng.

## 7. Wrong / tempting fixes
- Tăng startup probe timeout: chỉ che triệu chứng và kéo dài rollout.
- Xóa warmup: khiến startup nhanh nhưng phá contract dữ liệu.
- `Task.Run` không giới hạn cho mọi warmup: dễ tăng ThreadPool contention; chỉ cân nhắc khi có capacity và ownership rõ.
- Đánh dấu ready ngay sau host start: sai nếu endpoint cần snapshot đã hoàn thành.

## 8. Production implications
Phân biệt **process started**, **can answer basic requests**, và **data ready**. Dùng telemetry cho warmup duration, error, cancellation; readiness phải phản ánh nhu cầu endpoint. Worker cần hủy được khi shutdown, tránh giữ deployment vô hạn.

## 9. Trade-offs
Yield sớm cải thiện startup latency nhưng có thể cho traffic tới trước khi dữ liệu sẵn sàng. Nếu dữ liệu là critical prerequisite, readiness gating mới là chính sách đúng. Với SDK legacy blocking, vẫn cần đánh giá ThreadPool, backpressure và timeout.

## 10. Senior engineer should notice
Đừng kết luận "host đã start = nghiệp vụ ready". Phải xác định lifecycle boundary, ownership của task, failure propagation, cancellation và behavior khác nhau giữa runtime versions. Thiết kế test có gate deterministic đáng tin hơn benchmark timing ngẫu nhiên.

## References
- [Microsoft Learn — Background tasks with hosted services](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services)
- [Microsoft Learn — BackgroundService behavior change in .NET 10](https://learn.microsoft.com/en-us/dotnet/core/compatibility/extensions/10.0/backgroundservice-executeasync-task)
- [Microsoft Learn — Task.Yield](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.yield)
