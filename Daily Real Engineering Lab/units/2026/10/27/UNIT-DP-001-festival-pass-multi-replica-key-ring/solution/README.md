# Reference Solution — SPOILER

Chỉ đọc sau khi reproduce, tự sửa và chạy `verify.ps1`.

## 1. Symptoms

Session token được mint thành công nhưng replica khác và process mới không thể `Unprotect` token; các phiên có vẻ mất hiệu lực sau scale-out/rolling restart.

## 2. Evidence

`mint` và `read` là các lệnh `dotnet run` độc lập (PID khác). Payload và purpose không đổi; các lệnh đọc trả exit 21. Trước khi sửa, `verify.ps1` không đạt positive case đầu tiên.

## 3. Root cause

`starter/KeyRingFactory.cs` tạo `EphemeralDataProtectionProvider` mỗi lần. Key ring chỉ nằm trong process; mỗi replica/process không có key đã phát hành token trước đó. `replicaId` và `keyRingDirectory` được truyền vào nhưng không tạo ra một ranh giới key material ổn định.

## 4. Why the fix works

[Reference implementation](KeyRingFactory.cs) dùng `DataProtectionProvider.Create` với cùng thư mục key ring bền vững và cùng application name. Những process mới có thể đọc cùng key material, nhưng purpose khác vẫn không giải mã được. `verify.ps1` chạy trên **starter** đã sửa, không đọc file solution.

## 5. How to verify

Chép/áp dụng ý tưởng vào `starter/KeyRingFactory.cs` rồi chạy `./verify.ps1`. Phải thấy hai positive case accepted, hai negative case rejected. Để so sánh có kiểm soát, chạy `./reproduce.ps1` trên starter gốc trước khi chỉnh sửa.

## 6. Alternatives

- Shared filesystem key ring: phù hợp lab và một số deployment; cần quyền truy cập, backup, availability, encryption at rest.
- Key ring trong storage tập trung có bảo vệ phù hợp: tiện cho nhiều replica nhưng thêm dependency, latency và chính sách truy cập.
- Giữ ephemeral keys: chỉ hợp lý cho payload không cần sống qua process/restart; không phù hợp session của hệ thống scale-out.

## 7. Wrong / tempting fixes

- **Sticky sessions:** giảm tần suất chuyển replica nhưng không xử lý restart, failover hoặc thay đổi routing.
- **Tăng session TTL:** không giúp khi thiếu key để giải mã.
- **Catch rồi tự cấp token mới:** che lỗi, có thể làm hỏng hợp đồng xác thực và trải nghiệm đăng nhập.
- **Tắt xác thực token:** làm mất bảo đảm integrity và confidentiality; không chấp nhận được.
- **Copy key thủ công giữa pod:** dễ sai lệch khi rotation và không có vòng đời vận hành tin cậy.

## 8. Production implications

Không sử dụng thư mục tạm hoặc ephemeral volume làm kho key production. Khi cấu hình persistent key repository, xem xét mã hóa key at rest, RBAC/ACL, rotation, recovery, isolation giữa môi trường, app discriminator ổn định, alert khi không thể đọc key và rollout không làm mất khả năng giải mã token cũ.

## 9. Trade-offs

Storage chia sẻ cải thiện tính liên tục phiên nhưng trở thành tài sản bảo mật nhạy cảm và phụ thuộc vận hành. Mã hóa key ring với một external key-management service có thêm chi phí và blast radius. Cần tách persistence, encryption at rest, authorization và backup thành các quyết định riêng.

## 10. Senior engineering judgment

Chứng minh cơ chế bằng test xuyên process, không chỉ cùng instance. Xây negative tests để tránh fix chức năng bằng cách làm lỏng security boundary. Xác định migration plan cho token đã phát hành với key ephemeral: **không thể phục hồi token cũ nếu key đã mất**; cần kế hoạch reauthentication và thông báo phù hợp. Thiết kế thử nghiệm rolling restart trước khi production rollout.
