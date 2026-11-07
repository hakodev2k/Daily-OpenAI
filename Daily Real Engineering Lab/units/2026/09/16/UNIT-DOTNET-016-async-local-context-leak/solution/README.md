# Reference Solution — chỉ xem sau khi đã tự thử

## 1. Symptoms
Request flow nhận đúng correlation id, nhưng detached work chạy sau đó vẫn thấy cùng id dù request-side value đã được clear.

## 2. Evidence
`AsyncLocal<T>` không chỉ là một static variable thông thường. Giá trị của nó gắn với logical execution context. Starter cho thấy việc clear ở caller sau khi work đã được tạo không làm thay đổi snapshot/context mà work đó quan sát.

## 3. Root cause
`Task.Run` mặc định mang logical execution context của caller sang queued work. Vì correlation id được lưu bằng `AsyncLocal<string?>`, detached work thừa hưởng request-scoped ambient state tại boundary tạo task.

## 4. Why the fix works
Tạo detached work trong một boundary không flow execution context, hoặc tốt hơn trong production là enqueue một explicit work item chỉ chứa dữ liệu business thực sự cần thiết. Ví dụ tối thiểu cho lab:

```csharp
Task<string?> detached;
using (ExecutionContext.SuppressFlow())
{
    detached = Task.Run(async () =>
    {
        await Task.Delay(25);
        return CorrelationContext.Current.Value;
    });
}
```

Request-side correlation vẫn hoạt động trong request flow; detached task không nhận ambient request value.

## 5. How to verify
Chạy `./verify.ps1`. Script chạy chính `starter/` mà learner sửa và yêu cầu detached value là `null`.

## 6. Alternative fixes
Trong service thực tế, ưu tiên queue explicit message/work item vào `Channel<T>`, broker hoặc hosted worker. Truyền các field cần thiết thay vì dựa vào ambient request context. Nếu background operation cần một correlation riêng, tạo correlation mới ở consumer boundary.

## 7. Wrong or misleading fixes
- Chỉ set `AsyncLocal` về `null` sau `Task.Run`: quá muộn đối với context đã flow vào task.
- Thêm `Task.Delay` để task chạy sau khi clear: scheduling timing không thay đổi semantic boundary đáng tin cậy.
- Xóa toàn bộ correlation logging: loại bỏ observability thay vì sửa ownership boundary.

## 8. Production implications
Ambient context hữu ích cho request-local tracing nhưng nguy hiểm khi lifetime của work vượt lifetime request. Detached fire-and-forget work còn có thêm vấn đề về shutdown, retry, durability và exception handling.

## 9. Trade-offs
Suppressing context flow là công cụ low-level và có thể làm mất những ambient values khác mà code thực sự cần. Explicit work contracts dễ reasoning hơn nhưng cần thêm queue/worker design.

## 10. What a Senior engineer should notice
Câu hỏi quan trọng không chỉ là “làm sao clear AsyncLocal”, mà là **operation nào sở hữu context nào và lifetime của context có khớp lifetime của work hay không**. Khi lifetime khác nhau, explicit boundary thường đáng tin cậy hơn ambient state.