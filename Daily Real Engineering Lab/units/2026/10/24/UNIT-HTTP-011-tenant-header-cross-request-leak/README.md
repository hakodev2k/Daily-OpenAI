# UNIT-HTTP-011 — Rò rỉ ngữ cảnh tenant giữa các outbound HTTP request

## Mục tiêu
Điều tra một lỗi isolation trong tích hợp HTTP đa tenant, thu thập bằng chứng từ request thực sự được gửi, đưa ra giả thuyết và sửa mà không phá vỡ hành vi tuần tự.

## Bối cảnh thực tế
Một dịch vụ .NET nhận yêu cầu đồng bộ đơn hàng của nhiều tenant và gửi chúng tới cổng logistics chung. Test tuần tự đều xanh; khi hai yêu cầu đến gần nhau, hệ thống đối tác đôi khi ghi nhận đơn hàng vào sai tenant. Sự cố có tác động phân quyền và tính toàn vẹn dữ liệu.

## Bạn cần làm gì
1. Chạy baseline và kịch bản có lịch điều phối xác định.
2. Ghi lại `X-Tenant-Id` mà fake partner quan sát cho từng order.
3. Nêu ít nhất hai giả thuyết, chỉ ra bằng chứng loại trừ.
4. Chỉ chỉnh sửa `starter/PartnerClient.cs`; giữ nguyên API và hành vi gửi HTTP.
5. Chạy verify; giải thích phạm vi ownership của ngữ cảnh theo từng request.

## Yêu cầu môi trường
.NET SDK 10.x, PowerShell 7+ hoặc Windows PowerShell 5.1; không cần Docker, NuGet ngoài, Azure hay mạng.

## Chạy nhanh
Tại thư mục unit:
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```
Nếu policy chặn script: `powershell -ExecutionPolicy Bypass -File ./scripts/reproduce.ps1` (chỉ dùng cho script đã tự kiểm tra nội dung).

## Cách reproduce vấn đề
`./scripts/reproduce.ps1` phải exit 0 **khi đã quan sát được lỗi starter**. Đây không phải bài kiểm tra fix. Kịch bản sử dụng tín hiệu đồng bộ, không dựa vào may rủi của scheduler.

## Những gì cần quan sát
- Baseline tuần tự: mỗi order có tenant tương ứng.
- Hai yêu cầu chồng lấp: so sánh expected/actual trong output.
- Fake handler ghi header ngay khi outbound request được gửi; không gọi mạng thật.
- Nếu output không như mong đợi, kiểm tra SDK và chạy lại `dotnet run --project ./starter/OutboundTenantLab.csproj -- reproduce`.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong starter.
4. Verify bằng `./scripts/verify.ps1` (starter ban đầu **phải fail**).
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: chỉ mở [solution/README.md](solution/README.md) sau khi tự điều tra và thử sửa.

## Expected Results
[Trước](expected-results/before.md) · [Sau](expected-results/after.md)

## Estimated Time
45–60 phút. Độ khó D4, maturity L2. Phân biệt rõ giả thuyết về thread-safety và contract của HTTP headers.
