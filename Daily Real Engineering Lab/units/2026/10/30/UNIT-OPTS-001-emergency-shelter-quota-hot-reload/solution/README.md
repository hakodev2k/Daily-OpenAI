# Reference Solution — chỉ đọc sau khi tự điều tra

## Symptoms
Ở cấu hình 4, request 3 giường được chấp nhận. Sau khi giảm quota xuống 2, request 3 vẫn được chấp nhận. Sau khi tăng lên 5, request 5 vẫn bị từ chối. `configuration["Quota:MaxBedsPerRequest"]` đã thay đổi nhưng kết quả business không theo kịp.

## Evidence
`Program.cs` cố ý giữ một `BedReservationService` singleton xuyên suốt sáu kiểm tra. `configuration.Reload()` kích hoạt change token; `IOptionsMonitor<QuotaOptions>` có thể tạo lại `CurrentValue`. Tuy nhiên worker có thể vẫn sử dụng một snapshot cũ.

## Root cause
Constructor của starter đọc `options.CurrentValue` **một lần** và lưu đối tượng `QuotaOptions` vào field `_policy`. `IOptionsMonitor` không tự động mutate đối tượng đã được consumer giữ trước đó. Change token làm monitor invalidation/rebind cho lần đọc mới, không cập nhật reference cũ.

## Why the fix works
Reference ở `solution/Program.cs` giữ `IOptionsMonitor<QuotaOptions>` và đọc `_options.CurrentValue` **tại thời điểm quyết định**. Mỗi request mới nhận policy mới nhất đã được reload. Nó cũng fail-closed nếu quota <= 0 để không chấp nhận cấu hình sai. Đây là lựa chọn phù hợp khi policy được phép thay đổi trong khi service sống lâu.

## How to verify
1. Chạy `reproduce.ps1` với starter gốc, lưu `REPRODUCED`.
2. Sửa `starter/Program.cs` (không chép đè file test).
3. Chạy `verify.ps1`: cả sáu case PASS, exit 0, có marker `VERIFIED`.
4. Đối chiếu code của bạn với `solution/Program.cs`.
5. Optional: `dotnet run --project solution/ShelterQuotaLab.csproj -c Release` để self-check reference (không thay thế verify starter).

## Alternative fixes
- `IOptionsSnapshot<T>` thích hợp với scoped services trong HTTP request, không phải một singleton worker được giữ suốt đời.
- Tạo một immutable policy snapshot mới và atomically swap reference khi change event đến, có thể phù hợp khi cần validate toàn bộ policy và phát hành version nhất quán.
- Restart khi đổi cấu hình có thể hợp lệ với chính sách deploy-only, nhưng không đáp ứng live-change requirement ở đây.

## Wrong / tempting fixes
- Tăng số replica: mỗi replica vẫn có thể giữ quota cũ; còn làm drift khó chẩn đoán hơn.
- Gọi `configuration.Reload()` thêm nhiều lần: monitor đã có cơ hội reload; consumer vẫn dùng object cũ.
- Resolve lại `BedReservationService` singleton từ DI container: singleton vẫn là cùng instance.
- Dùng `IOptions<T>.Value`: snapshot theo vòng đời options instance, không đảm bảo live update.

## Production implications & trade-offs
- Thay đổi policy giữa một batch có thể khiến hai quyết định trong cùng batch thấy hai version khác nhau. Nếu cần atomic policy per batch, chụp snapshot ở **đầu batch**, ghi version, rồi dùng nhất quán trong batch.
- Validate bounds và fail-closed cho cấu hình sai; phân biệt thay đổi hợp lệ với config injection.
- Log `policyVersion`/hash và thời điểm reload, không log dữ liệu nhạy cảm.
- Nguồn config thật (App Configuration, file, secret store) phải hỗ trợ reload token; thay đổi key đơn lẻ không đảm bảo mọi provider tự reload.
- Cấu hình mới chỉ ảnh hưởng quyết định **mới**; reservation đã commit cần quy trình bù trừ riêng.

## Senior engineer should notice
Khác biệt giữa "monitor thấy value mới" và "consumer có sử dụng value mới"; ranh giới nhất quán khi policy đổi giữa workflow; cần integration test reload trên service cùng instance và kiểm thử giảm/tăng quota, không chỉ startup.
