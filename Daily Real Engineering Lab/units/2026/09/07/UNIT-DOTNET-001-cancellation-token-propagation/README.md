# UNIT-DOTNET-001 — Cancellation Đã Xảy Ra Nhưng Downstream Work Vẫn Chạy

## Mục tiêu

Hiểu cancellation trong .NET là cooperative, biết cách propagate `CancellationToken` qua call chain và phân biệt request cancellation với timeout policy.

## Bối cảnh thực tế

Một API nhận request từ client và gọi downstream service. Client đã hủy request sau 150 ms, nhưng application vẫn giữ downstream work chạy đến hết 2 giây. Ở tải cao, các operation không còn người chờ vẫn tiếp tục tiêu thụ connection, memory và concurrency budget.

## Bạn cần làm gì

1. Chạy starter và reproduce việc caller đã cancel nhưng downstream vẫn hoàn thành.
2. Ghi hypothesis trước khi sửa.
3. Xác định cancellation bị mất ở boundary nào.
4. Sửa để downstream dừng sớm khi caller cancel.
5. Verify functional behavior và cancellation behavior.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
dotnet run --project ./starter/RealEngineeringLab.csproj
```

Starter phải cho thấy caller phát tín hiệu cancellation khoảng 150 ms nhưng downstream vẫn hoàn thành khoảng 2000 ms sau đó.

## Những gì cần quan sát

- `CancellationTokenSource.CancelAfter` chỉ phát tín hiệu cancellation.
- Work chỉ dừng nếu token được truyền tới operation hỗ trợ cancellation và operation đó quan sát token.
- Bắt `OperationCanceledException` ở boundary phù hợp là khác với nuốt cancellation ở mọi layer.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](./hints/hint-01.md)
- [Hint 2](./hints/hint-02.md)
- [Hint 3](./hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ xem sau khi đã tự điều tra.

[Reference Solution](./solution/README.md)

## Expected Results

Before: downstream hoàn thành dù caller đã cancel.

After: operation kết thúc bằng cancellation sớm, không chờ đủ 2 giây.

## Estimated Time

30–45 phút.
