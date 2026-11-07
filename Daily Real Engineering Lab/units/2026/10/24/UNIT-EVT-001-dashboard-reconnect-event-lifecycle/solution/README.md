# Reference Solution — SPOILER

Chỉ đọc sau khi reproduce và tự sửa starter.

## 1. Symptoms
Sau reconnect, dashboard xử lý một notification hai lần. Offline và Dispose vẫn ghi nhận sự kiện mới; steady-session và repeat-connect vẫn đúng.

## 2. Evidence
`reconnect`: expected 1, actual 2. `offline-session` và `disposed-session`: expected 0, actual 1. Mỗi case dùng feed mới, phát sự kiện số lần xác định nên loại trừ publisher tự phát trùng.

## 3. Root cause
`Connect()` đăng ký `_feed.Published += OnPublished`. `Disconnect()` chỉ đặt `_connected = false` mà không hủy event subscription. Publisher sống lâu hơn session, tiếp tục giữ delegate; reconnect thêm một subscription. Dispose gọi Disconnect nhưng không giải phóng handler.

## 4. Why the fix works
Reference giữ guard idempotent Connect và thêm `_feed.Published -= OnPublished` vào Disconnect. Invariant: connected có đúng một handler, disconnected/disposed không có handler. Add/remove dùng cùng receiver và method group.

## 5. How to verify
Sau khi sửa `starter/Program.cs`, chạy `./scripts/verify.ps1`: 5 PASS, `behavioral-mismatches=0`. Tham khảo độc lập: `dotnet run --project ./solution/EventReconnectLab.csproj -- verify`. Script verify chính thức **chỉ** chạy learner-editable starter.

## 6. Alternative fixes
Một event bus trả về `IDisposable` subscription token có thể làm ownership rõ hơn. Một app-wide dispatcher đăng ký một lần cũng có thể hợp lý nhưng thay đổi contract và đòi test phân phối theo session.

## 7. Wrong / tempting fixes
Deduplicate notification ID tại UI che callback leak và có thể mất event lặp hợp lệ. Xóa Received khi reconnect làm mất dữ liệu nhưng không thay đổi registrations. Guard Connect đã tồn tại, không giải quyết Disconnect → Connect. `GC.Collect()` không loại được strong reference từ publisher.

## 8. Production implications
Publisher sống lâu giữ session chết trong memory, gây tăng CPU và duplicate side effects. Theo dõi active subscriptions, callbacks/session và memory; tránh gắn session ID làm metric label cardinality cao.

## 9. Trade-offs
Subscribe/unsubscribe đối xứng đơn giản, không cần broker hoặc Redis. Nếu publisher gọi callback đồng thời đúng lúc Disconnect, unsubscribe không đợi callback đang chạy; production có thể cần synchronization hoặc cancellation theo contract.

## 10. Senior engineering observations
Phân biệt idempotent Connect với lifecycle symmetry; kiểm thử transition graph thay vì chỉ happy path. Reference coi Dispose như Disconnect để giữ scope; public API production có thể cần cấm reconnect sau Dispose bằng disposed guard và test bổ sung.

## Reference code
[Program.cs](Program.cs). So sánh behavior/invariant, không so khớp từng dòng.
