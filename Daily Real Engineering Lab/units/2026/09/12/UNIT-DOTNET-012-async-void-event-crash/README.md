# UNIT-DOTNET-012 — Event publisher báo thành công rồi process vẫn crash

## Mục tiêu

Điều tra một in-process event flow trong .NET nơi publisher đã bọc `try/catch`, log `Publish completed`, nhưng process vẫn có thể chết sau đó khi subscriber xử lý bất đồng bộ thất bại.

## Bối cảnh thực tế

Order worker phát một event nội bộ sau khi cập nhật trạng thái đơn hàng. Subscriber gửi notification bất đồng bộ. Team tin rằng exception đã được publisher kiểm soát vì lời gọi phát event nằm trong `try/catch`, nhưng production thỉnh thoảng restart process sau khi log publish thành công.

## Bạn cần làm gì

1. Chạy `reproduce.ps1`.
2. Quan sát thứ tự log và exit code.
3. Ghi hypothesis trước khi sửa.
4. Sửa code trong `starter/` để publisher có thể chờ subscriber hoàn tất và quan sát failure theo một async contract rõ ràng.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem solution.

## Yêu cầu môi trường

- .NET SDK 8.0.x
- PowerShell

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script PASS khi chứng minh publisher log thành công trước, sau đó process kết thúc bất thường vì failure phát sinh ở subscriber không đi qua error boundary mà publisher mong đợi.

## Những gì cần quan sát

- Kiểu delegate của event/subscriber.
- Caller có nhận được `Task` đại diện cho công việc subscriber hay không.
- Exception xuất hiện trước hay sau `Publish completed`.
- `try/catch` của publisher thực sự bao quanh phần execution nào.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify learner-editable `starter/`.
5. Sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

- [Reference Solution](solution/README.md)

## Expected Results

### Before
- Publisher có thể log `Publish completed`.
- Subscriber failure xuất hiện sau đó.
- Process kết thúc bất thường.

### After
- Publisher await toàn bộ subscriber work thuộc publish operation.
- Failure được quan sát tại error boundary của publisher.
- Process kết thúc bình thường và verification xác nhận failure đã được xử lý có chủ đích.

## Estimated Time

30–45 phút.
