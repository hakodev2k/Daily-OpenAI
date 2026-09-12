# Reference Solution

> Chỉ xem sau khi đã reproduce và thử fix.

## 1. Symptoms

Hai queries chạy riêng lẻ đều đúng, nhưng khi khởi chạy đồng thời trên cùng request thì EF Core có thể ném `InvalidOperationException` báo một operation thứ hai bắt đầu trước khi operation trước hoàn thành.

## 2. Evidence

Starter cố tình làm database reader giữ trạng thái async trong một khoảng ngắn. Khi query thứ hai bắt đầu trong khoảng overlap đó, failure xuất hiện ổn định. Database không cần bị quá tải để xảy ra lỗi.

## 3. Root cause

`DbContext` không hỗ trợ nhiều operations chạy song song trên cùng instance. Scoped lifetime trong web app thường tạo một context cho một request; scoped không đồng nghĩa với thread-safe. `Task.WhenAll` đã biến hai operations tuần tự thành hai operations overlap trên cùng execution boundary.

## 4. Why the fix works

Fix đơn giản nhất khi hai queries không cần parallelism thực sự là serialize chúng:

```csharp
public async Task<DashboardResult> LoadAsync()
{
    var orders = await db.Orders.AsNoTracking().ToListAsync();
    var alerts = await db.Alerts.AsNoTracking().ToListAsync();

    return new DashboardResult(orders.Count, alerts.Count);
}
```

Không còn hai database operations active cùng lúc trên một `DbContext`.

Nếu latency evidence chứng minh parallel queries là cần thiết, dùng các context độc lập, ví dụ qua `IDbContextFactory<AppDbContext>`:

```csharp
public sealed class DashboardService(IDbContextFactory<AppDbContext> factory)
{
    public async Task<DashboardResult> LoadAsync()
    {
        await using var ordersDb = await factory.CreateDbContextAsync();
        await using var alertsDb = await factory.CreateDbContextAsync();

        var ordersTask = ordersDb.Orders.AsNoTracking().ToListAsync();
        var alertsTask = alertsDb.Alerts.AsNoTracking().ToListAsync();

        await Task.WhenAll(ordersTask, alertsTask);
        return new DashboardResult(ordersTask.Result.Count, alertsTask.Result.Count);
    }
}
```

## 5. How to verify

Sửa `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

PASS khi có `Orders=2; Alerts=2`, không có concurrent-operation exception và exit code bằng 0.

## 6. Alternative fixes

- Serialize queries trên cùng context: ít complexity nhất, thường là lựa chọn mặc định.
- Dùng context độc lập cho từng parallel operation khi có evidence rằng parallelism mang lại lợi ích đủ lớn.
- Gộp/restructure query nếu data model cho phép giảm round-trip mà vẫn giữ semantics rõ ràng.

## 7. Wrong / tempting fixes

- Bọc query trong `Task.Run`: không làm `DbContext` trở thành thread-safe và còn tiêu tốn ThreadPool.
- Bắt rồi retry `InvalidOperationException`: không sửa concurrency contract bị vi phạm.
- Đăng ký `DbContext` singleton: làm scope/state sharing nguy hiểm hơn.
- Chỉ tăng database resources: symptom nằm ở client-side usage contract, không phải capacity của database.

## 8. Production implications

Bug kiểu này thường xuất hiện sau một "latency optimization" tưởng như vô hại. Nó đặc biệt dễ xảy ra khi developer nhìn thấy hai I/O operations độc lập và mặc định cho rằng `Task.WhenAll` luôn an toàn.

## 9. Trade-offs

Parallel database calls có thể giảm wall-clock latency trong một số trường hợp, nhưng đổi lại tăng connection usage, database concurrency, complexity và pressure lên downstream. Chỉ parallelize sau khi đo bottleneck và capacity.

## 10. What a Senior engineer should notice

Senior engineer cần phân biệt ba khái niệm: async, concurrency và thread-safety. `await` giúp non-blocking; nó không làm object dùng chung hỗ trợ concurrent operations. Tối ưu latency phải giữ đúng lifetime/concurrency contract của dependency.