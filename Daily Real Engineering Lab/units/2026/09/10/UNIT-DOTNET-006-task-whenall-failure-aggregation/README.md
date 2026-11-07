# UNIT-DOTNET-006 — Batch job báo thiếu lỗi khi nhiều dependency cùng fail

## Mục tiêu

Điều tra cách một batch asynchronous thu thập failure khi nhiều `Task` chạy song song cùng thất bại, sau đó sửa implementation để báo cáo đầy đủ các lỗi mà không làm thay đổi hành vi thành công.

## Bối cảnh thực tế

Một payroll reconciliation job gọi ba dependency độc lập cho cùng một kỳ lương. Trong một incident, hai dependency cùng lỗi nhưng log cuối cùng chỉ hiển thị một failure. Điều này khiến on-call engineer mất thời gian vì tưởng chỉ có một downstream bị sự cố.

## Bạn cần làm gì

1. Chạy starter và reproduce symptom.
2. Ghi lại số task thực sự faulted và số failure được report.
3. Đưa ra ít nhất hai hypothesis trước khi sửa code.
4. Sửa code trong `starter/` để report đầy đủ mọi failure của batch.
5. Giữ nguyên việc các dependency được chạy song song.
6. Chạy `verify.ps1`.
7. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell
- Không cần database, Docker hay external service.

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script sẽ xác nhận starter có hai dependency failure nhưng chỉ report một failure.

## Những gì cần quan sát

- Bao nhiêu dependency hoàn thành thành công.
- Bao nhiêu dependency faulted.
- Bao nhiêu failure xuất hiện trong report cuối cùng.
- Batch task đang ở trạng thái nào sau khi tất cả operation kết thúc.

Không suy luận root cause chỉ từ exception đầu tiên được in ra; hãy kiểm tra lifecycle của toàn bộ nhóm task.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thu thập evidence.
4. Thử fix trong `starter/`.
5. Verify learner-editable code path.
6. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ mở sau khi đã reproduce và thử fix.

[Reference Solution](solution/README.md)

## Expected Results

### Starter

- Có 2 dependency faulted.
- Report cuối cùng chỉ chứa 1 failure.
- Process kết thúc có kiểm soát để learner có thể đọc evidence.

### Sau khi sửa

- Có 2 dependency faulted.
- Report cuối cùng chứa đủ cả 2 failure.
- Dependency thành công vẫn được thực thi đúng.
- Các operation vẫn được bắt đầu song song.

Nếu không reproduce được, hãy chạy trực tiếp:

```powershell
dotnet run --project starter/BatchFailureLab.csproj
```

và kiểm tra output `FAULTED_TASKS` cùng `REPORTED_FAILURES`.

## Estimated Time

35–50 phút.
