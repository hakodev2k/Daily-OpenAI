# UNIT-REDIR-001 — Vé tàu hợp lệ nhưng partner trả 401 sau chuyển hướng

## Mục tiêu
Điều tra sự cố outbound HTTP chỉ xuất hiện khi đối tác chuyển hướng request; phân biệt authentication, redirect và network behavior bằng evidence, sau đó bảo toàn security boundary và tạo regression checks.

## Bối cảnh thực tế
API kiểm tra vé tàu điện gọi cổng xác thực đối tác. Lệnh kiểm tra trực tiếp hoạt động; cùng credential đó, đường đi qua gateway đôi lúc trả 401. Partner log cho thấy gateway nhận request, còn service phía sau không nhận được credential như kỳ vọng. Hệ thống local mô phỏng hai đường đi trong cùng một process.

## Bạn cần làm gì
1. Chạy starter và ghi status, số request tới endpoint đích, số request có credential hợp lệ.
2. Đề xuất ít nhất hai giả thuyết và xác định chặng nào thay đổi HTTP contract.
3. Chỉ chỉnh `starter/PartnerGatewayClient.cs`. Không sửa server/harness, không đưa token vào URL hoặc log.
4. Verify: direct 200, redirected 200, và chuyển hướng ra origin không tin cậy bị từ chối trước khi gửi.
5. Đánh giá trade-off giữa xử lý redirect thủ công và cấu hình partner endpoint trực tiếp.

## Yêu cầu môi trường
.NET SDK 8+ cùng .NET 8 runtime, PowerShell 5.1+/7+ (nếu dùng scripts). Không cần Docker, Azure, internet hay NuGet package ngoài .NET SDK. Harness dùng loopback Kestrel và cổng TCP ngẫu nhiên; nếu môi trường cấm bind loopback, chạy trên máy local cho phép mở listener.

## Chạy nhanh
Từ thư mục unit:

```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

Sau khi tự sửa `starter/PartnerGatewayClient.cs`:

```powershell
./scripts/verify.ps1
```

Tương đương không dùng PowerShell:

```sh
dotnet run --project starter/RedirectLab.csproj -c Release -- --mode reproduce
dotnet run --project starter/RedirectLab.csproj -c Release -- --mode verify
```

## Cách reproduce vấn đề
`reproduce.ps1` trả exit 0 **chỉ khi** triệu chứng ban đầu xuất hiện. Sau khi sửa đúng, reproduce sẽ trả exit 1 (không còn lỗi). `verify.ps1` kiểm tra **starter bạn đã sửa**, không chạy code reference.

## Những gì cần quan sát
- Status và endpoint cuối cùng của request đi qua gateway so với direct call.
- Chênh lệch số request vào service và số request được chấp nhận.
- Response chuyển hướng mang thông tin gì; request tiếp theo có giữ nguyên các header hay không?
- Khi target đổi origin, ranh giới tin cậy nào cần áp dụng?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ mở sau khi tự điều tra.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). Ghi chép vào [workspace](workspace/my-investigation.md). Tham khảo [investigation guide](docs/investigation-guide.md) khi cần.

## Estimated Time
70 phút.
