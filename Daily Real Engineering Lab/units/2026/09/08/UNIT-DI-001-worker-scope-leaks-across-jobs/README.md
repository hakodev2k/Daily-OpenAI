# UNIT-DI-001 — Worker xử lý nhiều job nhưng context bị dùng chung

## Mục tiêu

Điều tra một scheduled worker chạy nhiều job liên tiếp nhưng dữ liệu theo từng job lại bị giữ chung ngoài ý muốn.

## Bối cảnh thực tế

Một worker nội bộ xử lý các order reconciliation job theo batch. Mỗi job cần một operation context riêng để gắn correlation information và các dependency scoped theo từng lần xử lý. Trong môi trường test, ba job chạy liên tiếp đều hoàn tất nhưng log cho thấy chúng dùng cùng một context identity.

## Bạn cần làm gì

1. Chạy starter system và reproduce triệu chứng.
2. Ghi ít nhất 2 hypothesis trước khi sửa code.
3. Xác định boundary nào đang quyết định lifetime của context.
4. Sửa learner-editable code trong `starter/` để mỗi job có context độc lập.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Worker xử lý đủ cả ba job.
- Mỗi dòng log có `JobId` và `ContextId`.
- So sánh `ContextId` giữa các job.
- Không cần đo timing; đây là lỗi lifetime/processing boundary có tính deterministic.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

⚠️ Spoiler: chỉ mở sau khi đã tự reproduce và thử sửa.

- [Reference solution](solution/README.md)

## Expected Results

Before:
- cả ba job được xử lý;
- `ContextId` không thay đổi giữa các job.

After:
- cả ba job vẫn được xử lý;
- mỗi job có một `ContextId` khác nhau;
- không thay đổi business behavior của handler.

Nếu không reproduce được, chạy `dotnet --info`, xác nhận SDK 8.x tồn tại rồi chạy lại `./reproduce.ps1`.

## Estimated Time

25–40 phút.
