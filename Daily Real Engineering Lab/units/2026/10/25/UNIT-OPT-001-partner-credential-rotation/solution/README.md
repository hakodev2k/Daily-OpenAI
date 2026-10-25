# Reference Solution — chỉ xem sau khi tự reproduce và sửa

## Symptoms
Lần gửi đầu thành công. Sau khi đối tác chuyển từ v1 sang v2 rồi v3, cấu hình hiển thị đúng phiên bản nhưng gateway vẫn nhận Unauthorized. Control call trực tiếp đến partner tiếp tục Accepted.

## Evidence
`Program.cs` tạo duy nhất một singleton `PartnerGateway`. Configuration provider nhận giá trị mới và phát change notification qua `Reload()`. Kết quả trong `PartnerEmulator.Attempts` cho thấy request thực tế vẫn mang version v1.

## Root cause
Starter đọc `IOptions<PartnerOptions>.Value` tại constructor và giữ riêng `_credentialVersion` trong singleton. `IOptions<T>` không được thiết kế để phản ánh cập nhật cấu hình sau khi đã tạo giá trị. Dù provider reload, giá trị đã capture trong gateway không tự thay đổi.

## Why the fix works
Thay **duy nhất** `starter/PartnerGateway.cs` bằng nội dung [PartnerGateway.cs](PartnerGateway.cs). `IOptionsMonitor<PartnerOptions>` hỗ trợ singleton và lấy cấu hình mới sau change notification. Đọc `CurrentValue` tại ranh giới mỗi lần `Send`, sau đó dùng một local variable ổn định cho operation hiện tại. Không giữ snapshot xuyên nhiều lần gửi.

## How to verify
~~~powershell
./scripts/verify.ps1
~~~
Trên code đã sửa, phải có `VERIFICATION_PASSED` và exit code 0, kể cả lần rotation thứ hai. `reproduce.ps1` sẽ không còn khẳng định lỗi ban đầu.

## Alternative fixes
- Nếu service chỉ sống trong request HTTP, có thể cân nhắc `IOptionsSnapshot<T>` trong scoped service; không inject scoped snapshot vào singleton.
- Có thể subscribe `OnChange` để cập nhật state đồng bộ có kiểm soát, nhưng phải xử lý thread-safety, dispose subscription và race với request đang chạy.
- Đối với secret rotation thực tế, thường cần secret provider có cơ chế refresh phù hợp, giai đoạn overlap hai key và kiểm tra validation; không mặc định rằng mọi provider tự động reload.

## Wrong / tempting fixes
- Restart toàn bộ pod sau mỗi lần rotation: che triệu chứng, gây gián đoạn và không chứng minh hot-reload đúng.
- Đọc lại `IConfiguration` ở mọi tầng tùy tiện: có thể chạy nhưng phân tán contract và validation, làm khó test.
- Chuyển `PartnerGateway` sang transient chỉ để lấy lại options: không sửa hợp đồng options, có thể gây lifecycle và hiệu năng không cần thiết.
- Hard-code v2/v3: chỉ qua một tình huống, thất bại ở lần rotation tiếp theo.

## Production implications
- Lab dùng **credential version marker**, không phải secret. Không log API key/token thật; chỉ log version ID hoặc fingerprint không đảo ngược.
- Khi rotation nhiều instance, xác nhận refresh latency và cấu hình đồng nhất; monitor trong một process không giải quyết phân phối config giữa các pod.
- Khi credentials bị thu hồi tức thời, in-flight request dùng key cũ có thể bị 401; cần quy trình overlap, retry an toàn theo idempotency và deadline.
- Validation nên fail closed khi cấu hình thiếu, kiểm soát health/readiness riêng biệt; không che 401 bằng retry vô hạn.

## Trade-offs
Đọc `CurrentValue` theo operation đơn giản và phù hợp cho client singleton; đổi lại phải bảo đảm provider thực sự phát change token và giá trị options hợp lệ. `OnChange` hữu ích khi cần tái tạo resource đắt tiền nhưng làm tăng complexity. Snapshot theo request phù hợp scope HTTP, không phù hợp singleton lâu sống.

## What a Senior engineer should notice
Đặt ranh giới rõ giữa provider reload, options cache, service lifetime, request ownership và rollout policy. Tách bằng chứng "config đã đổi" khỏi "request thực tế dùng config mới". Thiết kế test có ít nhất hai lần rotation để bắt fix chỉ đúng một lần.
