# UNIT-FUNC-001 — Overlapping Reconciliation Runs

## Mục tiêu

Điều tra một scheduled reconciliation job chạy ổn trên một host nhưng tạo xử lý trùng khi deployment có nhiều host instance.

## Bối cảnh thực tế

Một Azure Functions-style timer job đối soát invoice theo từng cửa sổ thời gian. Sau khi hệ thống scale lên hai instance, dashboard đôi lúc ghi nhận cùng một reconciliation window được xử lý hai lần. Không có exception rõ ràng và mỗi instance riêng lẻ đều ghi log như một lần chạy hợp lệ.

Lab dùng local simulation để tái hiện cơ chế mà không yêu cầu Azure subscription.

## Bạn cần làm gì

1. Chạy starter và reproduce triệu chứng.
2. Thu thập evidence từ output của hai host giả lập.
3. Viết ít nhất ba hypothesis vào `workspace/my-investigation.md`.
4. Xác định boundary cần đảm bảo khi nhiều host cùng kích hoạt một logical schedule window.
5. Sửa code trong `starter/`.
6. Chạy `verify.ps1`.
7. Sau đó mới so sánh với reference solution.

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

- Hai host có bắt đầu cùng một schedule window hay không.
- Số business executions được ghi nhận cho window đó.
- Guard hiện tại có phạm vi hiệu lực đến đâu khi có nhiều process/instance.
- CPU hoặc timing có phải điều kiện bắt buộc để lỗi xuất hiện hay không.

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

[Spoiler — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results

**Before:** hai host cùng nhận một logical schedule window và business execution count cho window đó là 2.

**After:** hai host vẫn có thể cùng được kích hoạt, nhưng chỉ một host thực hiện reconciliation cho window; một window mới vẫn được xử lý bình thường.

Nếu không reproduce được, chạy trực tiếp:

```powershell
dotnet run --project starter -- --reproduce
```

và kiểm tra assertion cuối chương trình.

## Estimated Time

Khoảng 60 phút.