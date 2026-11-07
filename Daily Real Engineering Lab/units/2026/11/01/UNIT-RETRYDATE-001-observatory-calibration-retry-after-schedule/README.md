# UNIT-RETRYDATE-001 — Lịch retry của đối tác lệch sau phản hồi 429

## Mục tiêu

Điều tra lịch retry của một .NET worker tích hợp HTTP, xây dựng hypothesis từ dữ liệu quan sát, cải thiện policy và kiểm tra regression bằng case deterministic. Không sử dụng network hoặc Azure.

## Bối cảnh thực tế

Trạm quan sát thiên văn gửi kết quả hiệu chuẩn thiết bị lên cổng đối tác. Trong đợt đồng bộ đêm, đối tác trả `429 Too Many Requests` hoặc `503 Service Unavailable` kèm `Retry-After`. Đội vận hành phát hiện một số request được lên lịch lại quá sớm, một số request vẫn được chấp nhận dù không còn đủ retry budget. Thời gian còn lại cho mỗi batch tối đa 20–30 giây trong bộ dữ liệu thử nghiệm.

## Bạn cần làm gì

1. Chạy starter, ghi lại case FAIL và case PASS.
2. Từ evidence, lập ít nhất hai hypothesis, chỉ rõ case nào bác bỏ hypothesis nào.
3. Chỉnh sửa **chỉ `starter/Program.cs`**, bảo toàn status policy, fallback và budget.
4. Chạy verification, đối chiếu với reference solution sau khi đã tự sửa.
5. Viết trade-off: clock skew, malformed header, cancellation, retry storm, idempotency.

## Yêu cầu môi trường

- .NET SDK 8.x, PowerShell 7+ hoặc Windows PowerShell 5.1.
- Không cần NuGet package ngoài, database, Docker, tài khoản cloud hay kết nối mạng khi SDK đã cài.

## Chạy nhanh

```powershell
pwsh ./scripts/run.ps1
pwsh ./scripts/reproduce.ps1
# Sau khi sửa starter/Program.cs:
pwsh ./scripts/verify.ps1
```

Nếu chạy Windows PowerShell: `powershell -File ./scripts/reproduce.ps1`. Nếu không dùng PowerShell: `dotnet run --project ./starter/RetryPlannerLab.csproj -- reproduce`, sau đó `-- verify`.

## Cách reproduce vấn đề

Chạy `./scripts/reproduce.ps1` **trước khi sửa**. Script exit code 0 khi tái hiện đúng nhóm lỗi kỳ vọng; exit code 2 nếu không tái hiện. Chạy `./scripts/verify.ps1` trên chính starter đã sửa; exit code 0 chỉ khi toàn bộ contract pass, exit code 3 nếu còn lỗi.

## Những gì cần quan sát

So sánh từng `status`, `header`, `retry`, `delay`, `expectedRetry`, `expectedDelay`. Xem [dữ liệu đối tác](evidence/partner-responses.md) và phân nhóm các response. Quan sát cả trường hợp không retry vì hết budget và response không được phép retry. Không kết luận từ một case duy nhất.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

Điền kết quả vào [workspace/my-investigation.md](workspace/my-investigation.md).

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler — chỉ xem sau khi tự reproduce và thử sửa:** [solution/README.md](solution/README.md). Mã mẫu nằm trong `solution/Program.cs`; có thể kiểm tra độc lập bằng `dotnet run --project ./solution/RetryPlannerSolution.csproj -- verify`.

## Expected Results

[Before](expected-results/before.md) · [After](expected-results/after.md). Không dựa vào thời gian thực; harness dùng đồng hồ cố định.

## Estimated Time

55 phút (10 reproduce/evidence, 15 hypotheses, 20 sửa và verify, 10 review/trade-offs).
