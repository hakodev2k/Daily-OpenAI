# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms

`IConfiguration` hiển thị endpoint mới sau reload nhưng `RoutingClient` vẫn trả endpoint ban đầu.

## 2. Evidence

Starter in ba mốc. `CONFIG_AFTER` chứng minh configuration provider đã nhận giá trị mới; sự khác biệt nằm giữa provider và consumer.

## 3. Root cause

`RoutingClient` nhận `IOptions<RoutingOptions>` rồi lưu `options.Value`. `IOptions<T>` cung cấp một options instance được tính và cache; singleton service vì vậy giữ snapshot hiệu dụng từ thời điểm resolve thay vì đọc giá trị mới sau reload.

## 4. Why the fix works

`IOptionsMonitor<T>` được thiết kế cho singleton dependencies và hỗ trợ change notifications/reload. Đọc `CurrentValue` tại thời điểm request cho phép consumer quan sát options instance mới sau configuration reload.

## 5. How to verify

Chạy:

```powershell
./verify.ps1
```

Kỳ vọng `REQUEST_BEFORE=https://primary.internal` và `REQUEST_AFTER=https://failover.internal`.

## 6. Alternative fixes

- Inject `IConfiguration` và đọc key mỗi lần: hợp lệ với configuration rất nhỏ nhưng làm mất strongly typed options boundary.
- Dùng `IOptionsSnapshot<T>` cho scoped consumer trong request-based application: phù hợp khi cần snapshot mới theo scope, nhưng không inject được vào singleton.
- Recreate service khi configuration thay đổi: đôi khi cần nếu dependency thực sự phải được rebuild, nhưng phức tạp hơn cho trường hợp chỉ đọc endpoint.

## 7. Wrong / tempting fixes

- Restart process sau mọi config change: che lifetime mismatch và phá mục tiêu runtime reload.
- Chuyển `RoutingClient` thành transient mà vẫn dùng `IOptions<T>`: lifetime của consumer thay đổi nhưng cached options semantics vẫn không giải quyết đúng contract reload.
- Poll file/config source thủ công: thêm complexity khi options infrastructure đã cung cấp change handling.

## 8. Production implications

Dynamic configuration chỉ an toàn khi consumer contract nói rõ giá trị nào có thể thay đổi lúc runtime. Với endpoint, credential, timeout hoặc feature configuration, cần cân nhắc consistency giữa các request và hành vi của dependency đang giữ connection/state.

## 9. Trade-offs

`IOptionsMonitor<T>` cho giá trị mới nhất nhưng một operation dài có thể đọc hai phiên bản nếu gọi `CurrentValue` nhiều lần. Khi operation cần consistency, đọc một lần ở operation boundary rồi dùng local snapshot cho toàn operation.

## 10. What a Senior engineer should notice

Configuration reload là chuỗi contract: source phát change → provider reload → options binding invalidates cache → consumer lifetime cho phép quan sát giá trị mới. Chứng minh từng boundary bằng evidence thay vì kết luận rằng “config reload không hoạt động”.
