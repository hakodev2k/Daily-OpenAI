# UNIT-HMACRAW-001 — Webhook trả xe đạp bị từ chối sau khi đối tác đổi định dạng JSON

## Mục tiêu
Điều tra lỗi xác thực webhook từ evidence, xây dựng hypothesis, sửa code và kiểm chứng cả positive lẫn negative cases.

## Bối cảnh thực tế
Hệ thống bike-share nhận biên nhận trả xe có chữ ký từ đối tác. Sau một bản cập nhật serializer, nhiều biên nhận bị từ chối dù schema, key và trạng thái HTTP không đổi. Một số payload vẫn hoạt động.

## Bạn cần làm gì
1. Chạy starter và thu thập bằng chứng cho ít nhất ba hypothesis.
2. So sánh từng mẫu JSON, chữ ký và kết quả xác thực.
3. Chỉ sửa `starter/WebhookVerifier.cs`; không sửa contract test.
4. Chạy verify và phân tích rủi ro production trước khi xem reference.

## Yêu cầu môi trường
.NET SDK 8.0.x, PowerShell. Không cần Azure, Docker, NuGet ngoài hay key thật.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```
Có thể dùng `dotnet run --project starter/BikeWebhookLab.csproj -- reproduce`.

## Cách reproduce vấn đề
`./scripts/reproduce.ps1` exit **0** khi lỗi starter được tái hiện. Sau khi tự sửa, `./scripts/verify.ps1` exit **0** khi tất cả contract pass. Hai script có mục đích khác nhau.

## Những gì cần quan sát
- `compact-valid`, `pretty-valid`, `reordered-valid`, `tampered-invalid`.
- Sự khác nhau giữa biểu diễn của thông điệp và ý nghĩa nghiệp vụ.
- Tỷ lệ rejected theo kiểu payload, không chỉ theo HTTP status.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis tại [workspace](workspace/my-investigation.md).
3. Thử fix.
4. Verify.
5. Chỉ sau đó xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Chỉ xem sau khi tự điều tra](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
65 phút.
