# UNIT-BG-002 — Worker Reports Success Before Work Is Done

## Mục tiêu

Điều tra một background worker có log `batch completed` nhưng một số item không được xử lý đầy đủ và lỗi từ item handler không xuất hiện ở failure path mong đợi.

## Bối cảnh thực tế

Một internal notification worker nhận một batch nhỏ và dispatch từng item. Trong production, dashboard đôi khi ghi nhận batch thành công trước khi toàn bộ notification hoàn tất. Khi một notification handler thất bại, batch vẫn có thể được đánh dấu thành công.

## Bạn cần làm gì

1. Chạy starter và reproduce triệu chứng.
2. Ghi hypothesis trước khi sửa.
3. So sánh thời điểm `batch completed`, item completion và exception observation.
4. Sửa `starter/` để completion contract phản ánh đúng công việc bắt buộc.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Thứ tự các event được ghi lại.
- Trạng thái batch khi một item handler chưa hoàn tất.
- Batch có phản ánh failure của item bắt buộc hay không.
- Không có external service; scenario là deterministic.

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

[Spoiler — chỉ xem sau khi tự thử](solution/README.md)

## Expected Results

**Before:** batch có thể công bố completion trước item work và không surface failure theo contract.

**After:** completion/failure của batch chỉ được quyết định sau khi toàn bộ required item work đạt terminal state.

Nếu không reproduce được, chạy `dotnet run --project starter/Starter.csproj -- --reproduce`.

## Estimated Time

35–50 phút.