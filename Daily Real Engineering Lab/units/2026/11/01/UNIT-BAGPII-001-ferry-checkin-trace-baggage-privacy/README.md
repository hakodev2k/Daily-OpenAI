# UNIT-BAGPII-001 — Dữ liệu hành khách xuất hiện trong trace của đối tác

## Mục tiêu

Điều tra một sự cố observability có ảnh hưởng đến quyền riêng tư, tái hiện bằng .NET 8 và sửa đúng ranh giới dữ liệu mà vẫn giữ được khả năng correlation.

## Bối cảnh thực tế

Dịch vụ check-in phà liên vùng vừa bật distributed tracing để giảm thời gian điều tra lỗi. Hệ thống vẫn xử lý check-in thành công, nhưng đội bảo mật phát hiện dữ liệu hành khách trong bản ghi telemetry của đối tác. Không có dấu hiệu thay đổi dữ liệu đặt chỗ.

## Bạn cần làm gì

1. Đọc [incident evidence](evidence/incident.md), phân loại dữ liệu nào được phép rời service.
2. Chạy starter, ghi ít nhất ba giả thuyết về nơi dữ liệu vượt qua trust boundary.
3. Reproduce triệu chứng; chỉnh **chỉ** `starter/TracePipeline.cs` để giảm rò rỉ mà vẫn giữ trace correlation.
4. Chạy verify trên starter đã chỉnh; chứng minh không làm mất nghiệp vụ check-in.
5. Đề xuất kiểm soát production, rồi mới xem reference solution.

## Yêu cầu môi trường

.NET SDK 8.x và PowerShell 7 (hoặc Windows PowerShell 5.1). Không cần Azure, Docker, collector hay NuGet ngoài SDK. Fixture sử dụng dữ liệu hành khách **giả lập**.

## Chạy nhanh

Tại thư mục unit:

```powershell
pwsh -File ./scripts/setup.ps1
pwsh -File ./scripts/run.ps1
pwsh -File ./scripts/reproduce.ps1
```

Nếu dùng Windows PowerShell, thay `pwsh` bằng `powershell`.

## Cách reproduce vấn đề

`reproduce.ps1` chạy một check-in cố định và xác nhận có dữ liệu không nên xuất hiện ở simulated downstream trace. Đây là phép kiểm tra trạng thái **trước fix**; sau fix, reproduce có thể fail đúng thiết kế.

## Những gì cần quan sát

So sánh trace ID, thông tin correlation, log tại service, simulated outbound telemetry và kết quả nghiệp vụ. Ghi rõ dữ liệu nào là signal cần thiết, dữ liệu nào là noise và dữ liệu nào vượt trust boundary. Không dùng email/passport thật.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis trong [workspace](workspace/my-investigation.md).
3. Tự sửa starter.
4. Verify bằng `pwsh -File ./scripts/verify.ps1`.
5. Chỉ sau đó xem solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md).

## Reference Solution

**Spoiler:** [Reference Solution](solution/README.md) — chỉ đọc sau khi tự điều tra.

## Expected Results

[Before](expected-results/before.md) · [After](expected-results/after.md). Verify luôn dùng **starter**, không chạy reference solution.

## Estimated Time

60 phút: 10 phút reproduce, 20 phút evidence/hypothesis, 20 phút fix/verify, 10 phút trade-offs.
